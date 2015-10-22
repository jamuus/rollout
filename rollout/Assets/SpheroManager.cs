using System;
using System.Collections.Generic;

using UnityEngine;

public enum SpheroDirection
{
    North       = 0x01,
    East        = 0x02,
    South       = 0x04,
    West        = 0x08
}

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
    public string FriendlyName { get; set; }
    public Vector2 Velocity { get; set; }

    // RollSphero message format:
    //  + MessageType - 1 byte
    //  + Direction   - 1 byte
    //  + Force       - 4 bytes 
    //  + DeviceName  - 1 + n bytes
    public void Roll(SpheroDirection direction, float force)
    {
        Server.Message message = new Server.Message(Server.MessageType.RollSphero);
        message.AddContent((byte)direction);
        message.AddContent(force);
        message.AddContent(DeviceName);
        Server.Send(message);
    }
}