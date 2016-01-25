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
}

public class Spectator
{
    public IPEndPoint Target { get; set; }

    public Spectator(IPEndPoint target)
    {
        Target = target;
    }
}