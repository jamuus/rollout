using System;
using System.Collections.Generic;
using System.Net;

public static class SpectatorManager
{
    public static List<Spectator> Instances { get; private set; }

    static SpectatorManager()
    {
        Instances = new List<Spectator>();
    }

    public static void SendToAll(ServerMessage message)
    {
        byte[] data = message.Compile();
        foreach (Spectator s in Instances)
            Server.Send(data, s.Target);
    }

    public static void SendNewEvents()
    {
        // Hardcode events for now.
        byte[] eventIds = new byte[] { 0, 4 };

        // Set up message.
        ServerMessage message = new ServerMessage(ServerMessageType.SetEvents);
        foreach (byte id in eventIds)
            message.AddContent(id);
        message.AddContent(10000); // Countdown time in ms.

        SendToAll(message);
    }
}

public class Spectator
{
    public IPEndPoint Target { get; set; }

    public Spectator(IPEndPoint target)
    {
        Target = target;
    }
}
