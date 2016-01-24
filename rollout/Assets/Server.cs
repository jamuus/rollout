using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

public enum ServerMessageType
{
    Test            = 0x00,
    RemoveSphero    = 0x01,
    SetEndianness   = 0x02,
    UpdateState     = 0x04,
    RollSphero      = 0x08,
    ServerDiscover  = 0x10,
    SpheroShoot     = 0x20,
    SpheroPowerUp   = 0x40,
    PauseGame       = 0x80,
    NodeInit        = 0x11,
    AppInit         = 0x21
}

public class ServerMessage
{
    public ServerMessageType    Type   { get; set; }
    public IPEndPoint           Target { get; set; }

    private List<byte> data;

    public ServerMessage()
    {
        data = new List<byte>();
        // Add placeholder for type. Faster when compiling.
        data.Add(0);
    }

    public ServerMessage(ServerMessageType type) :
        this()
    {
        Type = type;
    }

    public void AddContent(string content)
    {
        data.Add((byte)(content.Length & 0xff));
        data.AddRange(Encoding.ASCII.GetBytes(content));
    }

    public void AddContent(byte content)
    {
        data.Add(content);
    }

    public void AddContent(byte[] content)
    {
        data.AddRange(content);
    }

    public void AddContent(bool content)
    {
        data.Add(Convert.ToByte(content));
    }

    public void AddContent(float content)
    {
        data.AddRange(BitConverter.GetBytes(content));
    }

    public void AddContent(double content)
    {
        data.AddRange(BitConverter.GetBytes(content));
    }

    public void AddContent(int content)
    {
        data.AddRange(BitConverter.GetBytes(content));
    }

    public byte[] Compile()
    {
        byte[] bytes = data.ToArray();
        bytes[0] = (byte)Type;
        return bytes;
    }
}

public static class Server
{
    private static UdpClient    udpIncoming;
    private static UdpClient    udpOutgoing;
    private static Thread       listenThread;
    private static IPEndPoint   nodeServerTarget;

    public static string Name { get; set; }

    static Server()
    {
        Name = "Default Server Name";
    }

    public static void StartListening(int port)
    {
        udpIncoming = new UdpClient();
        udpIncoming.Client.SetSocketOption(SocketOptionLevel.Socket,
            SocketOptionName.ReuseAddress, true);
        udpIncoming.Client.ReceiveBufferSize = 2048;
        udpIncoming.Client.Bind(new IPEndPoint(IPAddress.Any, port));

        udpOutgoing = new UdpClient();
        udpOutgoing.Client.SetSocketOption(SocketOptionLevel.Socket,
            SocketOptionName.ReuseAddress, true);
        udpOutgoing.Client.Bind(new IPEndPoint(IPAddress.Any, port + 1));

        listenThread = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpIncoming.Receive(ref senderEndPoint);

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    ProcessReceivedBytes(data, senderEndPoint);
                }).Start();
            }
        });
        listenThread.Start();

        Debug.LogFormat("[Server] Started \"{0}\" successfully, listening on port {1}.",
            Name, port);
    }

    public static void StopListening()
    {
        listenThread.Abort();
        Debug.LogFormat("[Server] Stopped successfully.");
    }

    public static void Send(ServerMessage message)
    {
        byte[] bytes = message.Compile();
        udpOutgoing.Send(bytes, bytes.Length, message.Target);
    }

    public static void Send(byte[] bytes, IPEndPoint target)
    {
        udpOutgoing.Send(bytes, bytes.Length, target);
    }

    private static void ProcessReceivedBytes(byte[] bytes, IPEndPoint receivedFrom)
    {
        string prefix = string.Format("[Server] {0} - ", receivedFrom.ToString());

        if (!Enum.IsDefined(typeof(ServerMessageType), (int)bytes[0]))
        {
            Debug.LogFormat("{0} Unknown (0x{1:x2}).", prefix, bytes[0]);
            return;
        }

        ServerMessage message = new ServerMessage();
        ServerMessageType type = (ServerMessageType)bytes[0];

        Debug.LogFormat("{0} {1} (0x{2:x2}).", prefix, type.ToString(), bytes[0]);

        ++receivedFrom.Port; // TODO might have to find a better way to do this.

        switch (type)
        {
        case ServerMessageType.Test:
            break;
        case ServerMessageType.RemoveSphero:
            break;
        case ServerMessageType.SetEndianness:
            message.Type = ServerMessageType.SetEndianness;
            message.Target = receivedFrom;
            message.AddContent(BitConverter.IsLittleEndian);
            Send(message);
            break;
        case ServerMessageType.UpdateState:
            // Parse state, assumed to be received from Node.js server.
            SpheroManager.ParseUpdatedState(bytes, 1);
            break;
        case ServerMessageType.RollSphero:
            SpheroManager.Roll(bytes);
            break;
        case ServerMessageType.ServerDiscover:
            message.Type = ServerMessageType.ServerDiscover;
            message.Target = receivedFrom;
            message.AddContent(Name);
            Send(message);
            break;
        case ServerMessageType.SpheroShoot:
            SpheroManager.Shoot(bytes);
            break;
        case ServerMessageType.SpheroPowerUp:
            SpheroManager.UsePowerUp(bytes);
            break;
        case ServerMessageType.PauseGame:
            break;
        case ServerMessageType.NodeInit:
            // When node identifies itself, send the endianness.
            nodeServerTarget = receivedFrom;
            message.Type = ServerMessageType.SetEndianness;
            message.Target = nodeServerTarget;
            message.AddContent(BitConverter.IsLittleEndian);
            Send(message);
            break;
        case ServerMessageType.AppInit:
            Sphero sphero = null;

            message.Type = ServerMessageType.AppInit;
            message.Target = receivedFrom;
            message.AddContent(BitConverter.IsLittleEndian);

            if (BitConverter.ToBoolean(bytes, 1) && ((sphero = SpheroManager.GetNextSphero()) != null))
            {
                message.AddContent(sphero.DeviceName);
            }
            else
            {
                message.AddContent(SpheroManager.SpectatorName);
                SpectatorManager.Instances.Add(new Spectator(receivedFrom));
            }

            Send(message);

            if (sphero != null)
            {
                sphero.ControllerTarget = receivedFrom;
                sphero.HasController = true;
                sphero.SendStateToController();
            }
            break;
        }
    }
}