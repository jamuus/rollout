using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using UnityEngine;

public static class SpheroManager
{
    public static Dictionary<string, Sphero> Instances { get; private set; }
    public static PlayerControl ybr;
    public static PlayerControl boo;

    public static string SpectatorName { get { return "Spectator"; } }

    static SpheroManager()
    {
        Instances = new Dictionary<string, Sphero>();

        Debug.LogFormat("{0}, {1}", ybr, boo);
    }

    public static void Initialise()
    {

        ybr = GameObject.Find("player2").GetComponent<PlayerControl>();
        boo = GameObject.Find("player1").GetComponent<PlayerControl>();
    }

    public static Sphero GetNextSphero()
    {
        Debug.LogFormat("GET start");
        foreach (KeyValuePair<string, Sphero> sphero in Instances)
            if (!sphero.Value.HasController)
                return sphero.Value;
        Debug.LogFormat("GET end");
        return null;
    }

    public static string GetNextSpheroName()
    {
        Sphero s = GetNextSphero();
        return (s == null) ? SpectatorName : s.DeviceName;
    }

    public static void RemoveSphero(byte[] bytes)
    {
        string name = Encoding.ASCII.GetString(bytes, 1, bytes[0]);
        Instances[name].Leave();
    }

    public static void SendStateToControllers()
    {
        foreach (KeyValuePair<string, Sphero> sphero in Instances)
            if (sphero.Value.HasController)
                sphero.Value.SendStateToController();
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
            if (!Instances.TryGetValue(deviceName, out sphero)) {
                Instances.Add(deviceName, new Sphero());
                sphero = Instances[deviceName];
                if (deviceName.ToUpper().Contains("YBR")) {
                    ybr.sphero = sphero;
                    Debug.LogFormat("ybr - {0}", deviceName);
                } else {
                    boo.sphero = sphero;
                    Debug.LogFormat("boo - {0}", deviceName);
                }

            }

            sphero.DeviceName = deviceName;
            sphero.Velocity = velocity;
            sphero.Position = position;
            sphero.BatteryVoltage = voltage;
        }
    }

    public static void Roll(byte[] data)
    {
        int offset = 0;
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
        int offset = 0;
        byte weaponID = data[offset];
        ++offset;
        float direction = BitConverter.ToSingle(data, offset);
        offset += 4;
        string name = Encoding.ASCII.GetString(data, offset + 1, data[offset]);

        //Shoot from the relevant sphero
        Instances[name].Shoot(SpheroWeaponType.Default, direction);

        Debug.LogFormat("Sphero {0} firing weapon with ID {1} in direction {2}.", name, weaponID, direction);
    }

    public static void UsePowerUp(byte[] data)
    {
        int offset = 0;
        byte powerUpID = data[offset];
        ++offset;
        string name = Encoding.ASCII.GetString(data, offset + 1, data[offset]);

        // TODO

        Debug.LogFormat("Sphero {0} using power up with ID {1}.", name, powerUpID);
    }
}

public class Sphero
{
    public string               DeviceName              { get; set; }
    public Vector2              Velocity                { get; set; }
    public Vector2              Position                { get; set; }
    public float                BatteryVoltage          { get; set; }
    //public IPEndPoint           ControllerTarget        { get; set; }
    public float                Health                  { get; set; }
    public float                Shield                  { get; set; }
    public List<SpheroWeapon>   Weapons                 { get; set; }
    public List<SpheroPowerUp>  PowerUps                { get; set; }
    public bool                 HasController           { get; set; }
    public TcpConnection        Connection              { get; set; }
    public PlayerControl        UnityObject             { get; set; }
    public ProjectileControl    UnityProjectileControl  { get; set; }

    public Sphero()
    {
        Weapons = new List<SpheroWeapon>();
        PowerUps = new List<SpheroPowerUp>();

        Weapons.Add(new SpheroWeapon(SpheroWeaponType.Default));

        HasController = false;
    }

    // RollSphero message format:
    //  + MessageType - 1 byte
    //  + Direction   - 4 bytes
    //  + Force       - 4 bytes
    //  + DeviceName  - 1 + n bytes
    public void Roll(float direction, float force)
    {
        /*ServerMessage message = new ServerMessage(ServerMessageType.RollSphero);

        message.Target = Server.NodeServerTarget;
        message.AddContent(direction);
        message.AddContent(force);
        message.AddContent(DeviceName);

        Server.Send(message);*/

    }

    public void Shoot(SpheroWeaponType type, float direction)
    {
        //Get the ProjectileControl object associated with the shooting player
        //String playerName = DeviceName.ToLower().Contains("boo") ? "player1" : "player2";
        //ProjectileControl playerProjectile = GameObject.Find(playerName).GetComponent<ProjectileControl>();

        //Put the direction into the correct range
        direction += (float)Math.PI;

        //Covert the direction into a vector
        Vector3 directionVector = new Vector3((float)Math.Cos(direction), 0.0f, (float)Math.Sin(direction));

        //Tell the sphero to shoot
        //playerProjectile.Shoot(directionVector);
        //UnityProjectileControl.Shoot(directionVector);
        Test.QueueOnMainThread(() =>
        {
            UnityProjectileControl.Shoot(directionVector);
        });

    }

    public void Leave()
    {
        Connection = null;
        HasController = false;
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

        Connection.Send(message);
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
    Something,
    SomethingElse
}

public class SpheroPowerUp
{
    public SpheroPowerUpType Type { get; set; }
    // TODO
}