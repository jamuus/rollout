using System;
using System.Collections.Generic;
using System.Net;

//using RobotKit;
using UnityEngine;

public static class SpheroManager
{
    public static Dictionary<string, Sphero> Instances { get; private set; }

    static SpheroManager()
    {
        Instances = new Dictionary<string, Sphero>();
    }
}

public class Sphero
{
    public string DeviceName { get; set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Position { get; set; }
    public float BatteryVoltage { get; set; }
    public IPEndPoint ControllerTarget { get; set; }
    public float Health { get; set; }
    public float Shield {get; set; }
    public List<SpheroWeapon> Weapons { get; set; }
    public List<SpheroPowerUp> PowerUps { get; set; }

    public Sphero()
    {
        Weapons = new List<SpheroWeapon>();
        PowerUps = new List<SpheroPowerUp>();

        Weapons.Add(new SpheroWeapon(SpheroWeaponType.Default));
    }
   
    // RollSphero message format:
    //  + MessageType - 1 byte
    //  + Direction   - 4 bytes
    //  + Force       - 4 bytes 
    //  + DeviceName  - 1 + n bytes
    public void Roll(float direction, float force)
    {
		/*
        Server.Message message = new Server.Message(Server.MessageType.RollSphero);
        message.AddContent(direction);
        message.AddContent(force);
        message.AddContent(DeviceName);
        Server.Send(message);*/

		ServerMessage message = new ServerMessage(ServerMessageType.RollSphero);
		message.AddContent(direction);
		message.AddContent(force);
		message.AddContent(DeviceName);
		Server.Send(message);
    }

    public void SendStateToController()
    {
        ServerMessage message = new ServerMessage(ServerMessageType.UpdateState);
        message.AddContent(DeviceName);
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