using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public static class SpheroManager
{
    public static Dictionary<string, Sphero> Instances { get; private set; }
    // public static PlayerControl ybr;
    // public static PlayerControl boo;

    public static PlayerControl[]   Players;
    public static int               NumberOfPlayers;
    public const int                MaxNumberOfPlayers = 2;

    public static string SpectatorName { get { return "Spectator"; } }

    static SpheroManager()
    {
        Instances = new Dictionary<string, Sphero>();

        Players = new PlayerControl[MaxNumberOfPlayers];
        NumberOfPlayers = 0;
    }

    public static void Initialise()
    {
        Players[0] = GameObject.Find("player1").GetComponent<PlayerControl>();
        Players[1] = GameObject.Find("player2").GetComponent<PlayerControl>();

        // ybr = GameObject.Find("player2").GetComponent<PlayerControl>();
        // boo = GameObject.Find("player1").GetComponent<PlayerControl>();
    }

    public static Sphero GetNextSphero()
    {
        foreach (KeyValuePair<string, Sphero> sphero in Instances)
            if (!sphero.Value.HasController)
                return sphero.Value;
        return null;
    }

    public static string GetNextSpheroName()
    {
        Sphero s = GetNextSphero();
        return (s == null) ? SpectatorName : s.DeviceName;
    }

    public static void RemoveSphero(byte[] bytes)
    {
        string name = Encoding.ASCII.GetString(bytes, 2, bytes[1]);
        Instances[name].Leave();
    }

    public static void SendStateToControllers()
    {
        foreach (KeyValuePair<string, Sphero> sphero in Instances)
            if (sphero.Value.HasController)
                sphero.Value.SendStateToController();
    }

    public static void RestartGame()
    {
        ServerMessage message = new ServerMessage(ServerMessageType.Restart);

        foreach (KeyValuePair<string, Sphero> sphero in Instances)
        {
            sphero.Value.Health = UniversalHealth.maxHealth;
            sphero.Value.PowerUps.Clear();
        }
    }

    public static void ParseUpdatedState(byte[] bytes, int index)
    {
        while (index < bytes.Length) {
            string deviceName = Encoding.ASCII.GetString(bytes, index + 1, bytes[index]);
            index += deviceName.Length + 1;

            Vector2 velocity = new Vector2();
            velocity.x = BitConverter.ToSingle(bytes, index);
            index += 4;
            velocity.y = BitConverter.ToSingle(bytes, index);
            index += 4;

            Vector2 position = new Vector2();
            position.x = BitConverter.ToSingle(bytes, index);
            index += 4;
            position.y = BitConverter.ToSingle(bytes, index);
            index += 4;

            // Debug.LogFormat("received data for {0}: ({1}, {2})", deviceName, position.x, position.y);
            float voltage = BitConverter.ToSingle(bytes, index);
            index += 4;

            Sphero sphero;
            if (!Instances.TryGetValue(deviceName, out sphero) && NumberOfPlayers < MaxNumberOfPlayers) {
                Instances.Add(deviceName, new Sphero());
                sphero = Instances[deviceName];

                int currentPlayerIndex = NumberOfPlayers++;
                Players[currentPlayerIndex].sphero = sphero;
                MainThread.EnqueueAction(() =>
                {
                    Players[currentPlayerIndex].sphero.UnityProjectileControl = Players[currentPlayerIndex].GetComponent<ProjectileControl>();
                    Debug.LogFormat("[SpheroManager] Added Sphero \"{0}\".", deviceName);
                });

                // if (deviceName.ToUpper().Contains("YBR")) {
                //     ybr.sphero = sphero;
                //     MainThread.EnqueueAction(() =>
                //     {
                //         ybr.sphero.UnityProjectileControl = ybr.GetComponent<ProjectileControl>();
                //         Debug.LogFormat("[SpheroManager] Added Sphero \"{0}\".", deviceName);
                //     });
                // } else {
                //     boo.sphero = sphero;
                //     MainThread.EnqueueAction(() =>
                //     {
                //         boo.sphero.UnityProjectileControl = boo.GetComponent<ProjectileControl>();
                //         Debug.LogFormat("[SpheroManager] Added Sphero \"{0}\".", deviceName);
                //     });
                // }
            }

            sphero.DeviceName = deviceName;
            sphero.Velocity = velocity;
            sphero.Position = position;
            sphero.BatteryVoltage = voltage;
        }
    }

    public static void Roll(byte[] data)
    {
        int offset = 1;
        float direction = BitConverter.ToSingle(data, offset);
        offset += 4;
        float force = BitConverter.ToSingle(data, offset);
        offset += 4;
        string name = Encoding.ASCII.GetString(data, offset + 1, data[offset]);

        // TODO Process with collisions etc, then forward on to node.

        Instances[name].Roll(direction, force);

        //Debug.LogFormat("Rolling Sphero {0} in direction {1} with force {2}.", name, direction, force);
    }

    public static void Shoot(byte[] data)
    {
        int offset = 1;
        byte weaponID = data[offset];
        ++offset;
        float direction = BitConverter.ToSingle(data, offset);
        offset += 4;
        string name = Encoding.ASCII.GetString(data, offset + 1, data[offset]);

        //Shoot from the relevant sphero
        Instances[name].Shoot(SpheroWeaponType.Default, direction);

        //Debug.LogFormat("Sphero {0} firing weapon with ID {1} in direction {2}.", name, weaponID, direction);
    }

    public static void UsePowerUp(byte[] data)
    {
        int offset = 1;
        byte powerUpID = data[offset];
        ++offset;
        string name = Encoding.ASCII.GetString(data, offset + 1, data[offset]);

        Instances[name].UsePowerUp(new SpheroPowerUp((SpheroPowerUpType)powerUpID));

        //Debug.LogFormat("Sphero {0} using power up with ID {1}.", name, powerUpID);
    }
}

public class Sphero
{
    public string                       DeviceName              { get; set; }
    public Vector2                      Velocity                { get; set; }
    public Vector2                      Position                { get; set; }
    public float                        BatteryVoltage          { get; set; }
    //public IPEndPoint                   ControllerTarget        { get; set; }
    public float                        Health                  { get; set; }
    public float                        Shield                  { get; set; }
    public List<SpheroWeapon>           Weapons                 { get; set; }
    public List<SpheroPowerUp>          PowerUps                { get; set; }
    public bool                         HasController           { get; set; }
    //public TcpConnection                Connection              { get; set; }
    public PlayerControl                UnityObject             { get; set; }
    public ProjectileControl            UnityProjectileControl  { get; set; }
    public TcpServerModule.Connection   Connection              { get; set; }
#if SOFTWARE_MODE
    public Vector2                      Force                   { get; set; }
#endif

    public Vector3                      MoveForce               { get; set; }
    public Vector3                      EnvironmentForce        { get; set; }

    public Sphero()
    {
        Weapons = new List<SpheroWeapon>();
        PowerUps = new List<SpheroPowerUp>();

        Weapons.Add(new SpheroWeapon(SpheroWeaponType.Default));

        HasController = false;

        MoveForce           = new Vector3(0, 0, 0);
        EnvironmentForce    = new Vector3(0, 0, 0);
    }

    // RollSphero message format:
    //  + MessageType - 1 byte
    //  + Direction   - 4 bytes
    //  + Force       - 4 bytes
    //  + DeviceName  - 1 + n bytes
    public void Roll(float direction, float force)
    {
#if SOFTWARE_MODE
        Force = new Vector2(force * Mathf.Sin(direction) * 10.0f, force * -Mathf.Cos(direction) * 10.0f);
#else
        force *= 10.0f;
        MoveForce = new Vector3(force * Mathf.Cos(direction), 0.0f, force * Mathf.Sin(direction));
        // ServerMessage message = new ServerMessage(ServerMessageType.RollSphero);

        // message.Target = Server.NodeServerTarget;
        // message.AddContent(direction);
        // message.AddContent(force);
        // message.AddContent(DeviceName);

        // Server.Send(message);
#endif

    }

    public void Shoot(SpheroWeaponType type, float direction)
    {
        //Get the ProjectileControl object associated with the shooting player
        //String playerName = DeviceName.ToLower().Contains("boo") ? "player1" : "player2";
        //ProjectileControl playerProjectile = GameObject.Find(playerName).GetComponent<ProjectileControl>();

        //Put the direction into the correct range
        direction += (float)Math.PI;

        //Covert the direction into a vector
#if SOFTWARE_MODE
        Vector3 directionVector = new Vector3(Mathf.Cos(direction), 0.0f, Mathf.Sin(direction));
#else
        Vector3 directionVector = new Vector3(Mathf.Cos(direction), 0.0f, Mathf.Sin(direction));
#endif

        //Tell the sphero to shoot
        //playerProjectile.Shoot(directionVector);
        //UnityProjectileControl.Shoot(directionVector);
        MainThread.EnqueueAction(() => {
            UnityProjectileControl.Shoot(directionVector);
        });

    }

    public void SendMove()
    {
        Vector3 resultant = MoveForce + EnvironmentForce;

        float force = Mathf.Clamp(resultant.magnitude, 0.0f, 0.3f);
        float direction = Mathf.Atan2(-resultant.x, resultant.z);

        //Debug.LogFormat("MV: {2} DIR: {0} FRC: {1}", direction, force, MoveForce);

        if (Server.NodeServerTarget != null)
        {
            ServerMessage message = new ServerMessage(ServerMessageType.RollSphero);
            message.Target = Server.NodeServerTarget;
            message.AddContent(direction);
            message.AddContent(force);
            message.AddContent(DeviceName);
            Server.Send(message);
        }
    }

    public void Leave()
    {
        Connection.Socket = null;
        HasController = false;
    }

    public void UsePowerUp(SpheroPowerUp powerUp)
    {
        if ((int)powerUp.Type < 100)
        {
            MainThread.EnqueueAction(() =>
            {
                UnityObject.UsePowerUp((int)powerUp.Type);
            });
        }
        else
        {
            MainThread.EnqueueAction(() =>
            {
                UnityProjectileControl.ChangeActiveWeapon((int)powerUp.Type);
            });
        }

        PowerUps.Remove(powerUp);
    }

    public void SendStateToController()
    {
        ServerMessage message = new ServerMessage(ServerMessageType.UpdateState);
        //message.AddContent(DeviceName);
        message.AddContent(Health);
        message.AddContent(Shield);
        message.AddContent(BatteryVoltage);
        message.AddContent((byte)Weapons.Count);

        foreach (SpheroWeapon weapon in Weapons)
            message.AddContent((byte)weapon.Type);

        message.AddContent((byte)PowerUps.Count);

        foreach (SpheroPowerUp powerup in PowerUps)
            message.AddContent((byte)powerup.Type);

        //message.Target = ControllerTarget;
        //Server.Send(message);

        /*byte[] data = BitConverter.GetBytes(Health);
        for (int i = 0; i < data.Length; ++i)
            Debug.LogFormat("Health: {0:x2}", data[i]);*/

        //Connection.Send(message);
        Server.SendTcp(Connection, message);
    }
}

public enum SpheroWeaponType : byte {
    Default = 0,
    RailGun = 1
}

public class SpheroWeapon
{
    public SpheroWeaponType Type { get; set; }
    public float Damage { get; set; }
    public float DropOff { get; set; }
    public bool IsProjectile { get; set; }
    // TODO more weapon stats.

    public SpheroWeapon(SpheroWeaponType type)
    {
        Type = type;
    }
}

public enum SpheroPowerUpType : byte {
    Boost           = 0,
    DamageEnemy     = 1,
    StunEnemey      = 2,
    SlowDownEnemy   = 3,
    Regeneration    = 4,
    Gun             = 100,
    Homing_Launcher = 101,
    Grenade         = 102,
}

public class SpheroPowerUp : IEquatable<SpheroPowerUp>
{
    public SpheroPowerUpType Type { get; set; }

    public SpheroPowerUp(SpheroPowerUpType type)
    {
        Type = type;
    }

    public bool Equals(SpheroPowerUp other)
    {
        if (other == null)
            return false;

        return Type == other.Type;
    }

    // TODO
}
