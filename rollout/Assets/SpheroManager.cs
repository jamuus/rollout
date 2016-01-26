using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using UnityEngine;

public static class SpheroManager
{
    public static Dictionary<string, Sphero> Instances { get; private set; }

    public static string SpectatorName { get { return "Spectator"; } }

    static SpheroManager()
    {
        Instances = new Dictionary<string, Sphero>();
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

    public static void ParseUpdatedState(byte[] bytes, int index)
    {
        while (index < bytes.Length)
        {
            string deviceName = Encoding.ASCII.GetString(bytes, index + 1, bytes[index]);
            index += deviceName.Length + 1;

            Vector2 velocity = new Vector2();
            velocity.x = BitConverter.ToSingle(bytes, index);
            index += 4;
            velocity.y = BitConverter.ToSingle(bytes, index);
            index += 4;

            Vector2 position = new Vector2();
            velocity.x = BitConverter.ToSingle(bytes, index);
            index += 4;
            velocity.y = BitConverter.ToSingle(bytes, index);
            index += 4;

            float voltage = BitConverter.ToSingle(bytes, index);
            index += 4;

            Sphero sphero;
            if (!Instances.TryGetValue(deviceName, out sphero))
            {
                Instances.Add(deviceName, new Sphero());
                sphero = Instances[deviceName];
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

        Debug.LogFormat("Rolling Sphero {0} in direction {1} with force {2}.", name, direction, force);
    }

    public static void Shoot(byte[] data)
    {
        int offset = 1;
        byte weaponID = data[offset];
        ++offset;
        float direction = BitConverter.ToSingle(data, offset);
        offset += 4;
        string name = Encoding.ASCII.GetString(data, offset + 1, data[offset]);

        // TODO

        Debug.LogFormat("Sphero {0} firing weapon with ID {1} in direction {2}.", name, weaponID, direction);
    }

    public static void UsePowerUp(byte[] data)
    {
        int offset = 1;
        byte powerUpID = data[offset];
        ++offset;
        string name = Encoding.ASCII.GetString(data, offset + 1, data[offset]);

        // TODO

        Debug.LogFormat("Sphero {0} using power up with ID {1}.", name, powerUpID);
    }
}

public class Sphero
{
    public string               DeviceName          { get; set; }
    public Vector2              Velocity            { get; set; }
    public Vector2              Position            { get; set; }
    public float                BatteryVoltage      { get; set; }
    public IPEndPoint           ControllerTarget    { get; set; }
    public float                Health              { get; set; }
    public float                Shield              { get; set; }
    public List<SpheroWeapon>   Weapons             { get; set; }
    public List<SpheroPowerUp>  PowerUps            { get; set; }
    public bool                 HasController       { get; set; }

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
		ServerMessage message = new ServerMessage(ServerMessageType.RollSphero);
        message.Target = Server.NodeServerTarget;
		message.AddContent(direction);
		message.AddContent(force);
		message.AddContent(DeviceName);
		Server.Send(message);
    }

    public void Shoot(SpheroWeaponType type, float direction)
    {
        // TODO
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

        message.Target = ControllerTarget;
        Server.Send(message);
    }
}

public enum SpheroWeaponType : byte
{
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

public enum SpheroPowerUpType : byte
{
    Something,
    SomethingElse
}

public class SpheroPowerUp
{
    public SpheroPowerUpType Type { get; set; }
    // TODO
}