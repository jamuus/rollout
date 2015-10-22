using System;
using System.Collections.Generic;

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
   
    // RollSphero message format:
    //  + MessageType - 1 byte
    //  + Direction   - 4 bytes
    //  + Force       - 4 bytes 
    //  + DeviceName  - 1 + n bytes
    public void Roll(float direction, float force)
    {
        Server.Message message = new Server.Message(Server.MessageType.RollSphero);
        message.AddContent(direction);
        message.AddContent(force);
        message.AddContent(DeviceName);
        Server.Send(message);
    }
}