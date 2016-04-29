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
    AppInit         = 0x21,
    SetEvents       = 0x22,
    VoteEvent       = 0x23,
    Restart         = 0x24
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

    public static int Length(ServerMessageType type)
    {
        switch (type)
        {
            case ServerMessageType.AppInit:
                return 2;
            case ServerMessageType.RollSphero:
                return 10;
            case ServerMessageType.SpheroShoot:
                return 7;
            case ServerMessageType.SpheroPowerUp:
                return 3;
            case ServerMessageType.RemoveSphero:
                return 2;
            default:
                return -1;
        }
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
    private static UdpClient            udpIncoming;
    private static UdpClient            udpOutgoing;
    private static TcpListener          tcpListener;
    private static Thread               udpListenThread;
    private static TcpServerModule      tcpServer;
    private static bool                 udpListening;

    public static List<TcpServerModule.Connection>  Connections      { get { return tcpServer.Connections; } }
    public static IPEndPoint                        NodeServerTarget { get; private set; }
    public static string                            Name             { get; set; }

    static Server()
    {
        Name = "Default Server Name";
        udpListening = false;
    }

    public static void StartListening(int port)
    {
        udpIncoming = new UdpClient();
        udpIncoming.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpIncoming.Client.ReceiveBufferSize = 2048;
        udpIncoming.Client.Bind(new IPEndPoint(IPAddress.Any, port));

        udpOutgoing = new UdpClient();
        udpOutgoing.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpOutgoing.Client.Bind(new IPEndPoint(IPAddress.Any, port + 1));

        udpListening = true;
        udpListenThread = new Thread(() =>
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                while (udpListening)
                {
                    IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpIncoming.Receive(ref senderEndPoint);

                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        ProcessReceivedBytes(data, senderEndPoint);
                    }).Start();
                }
            }
            catch
            {
                Debug.LogFormat("[Server] UDP Listen exception.");
            }
        });

        tcpServer = new TcpServerModule(port);
        tcpServer.DataReceived += TcpDataReceived;
        tcpServer.Start();

        udpListenThread.Start();

        Debug.LogFormat("[Server] Started \"{0}\" successfully, listening on port {1}.", Name, port);
    }

    public static void StopListening()
    {
        tcpServer.Stop();

        udpIncoming.Close();
        udpListening = false;
        udpListenThread.Join();

        //udpListenThread.Abort();

        Debug.LogFormat("[Server] Stopped successfully.");
    }

    private static void TcpDataReceived(object sender, SocketAsyncEventArgs args)
    {
        TcpServerModule.Connection connection = args.UserToken as TcpServerModule.Connection;

        ServerMessage message = new ServerMessage();
        ServerMessageType type = (ServerMessageType)connection.Buffer[0];

        switch (type)
        {
            case ServerMessageType.AppInit:
                Sphero sphero = null;

                message.Type = ServerMessageType.AppInit;
                message.AddContent(BitConverter.IsLittleEndian);

                if (BitConverter.ToBoolean(connection.Buffer, 0) && ((sphero = SpheroManager.GetNextSphero()) != null))
                {
                    message.AddContent(sphero.DeviceName);
                }
                else
                {
                    message.AddContent(SpheroManager.SpectatorName);
                    //SpectatorManager.Instances.Add(new Spectator(receivedFrom));
                }

                tcpServer.Send(connection, message);

                if (sphero != null)
                {
                    sphero.HasController = true;
                    sphero.Connection = connection;
                    sphero.SendStateToController();
                }
                break;
            case ServerMessageType.RollSphero:
                SpheroManager.Roll(connection.Buffer);
                break;
            case ServerMessageType.SpheroShoot:
                SpheroManager.Shoot(connection.Buffer);
                break;
            case ServerMessageType.SpheroPowerUp:
                SpheroManager.UsePowerUp(connection.Buffer);
                break;
            case ServerMessageType.RemoveSphero:
                SpheroManager.RemoveSphero(connection.Buffer);
                tcpServer.Disconnect(connection);
                break;
            default:
                break;
        }

        connection.BufferOffset = 0;
        connection.BufferRemaining = 1;
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

    public static bool SendTcp(TcpServerModule.Connection connection, ServerMessage message)
    {
        return tcpServer.Send(connection, message);
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

        // Debug.LogFormat("{0} {1} (0x{2:x2}).", prefix, type.ToString(), bytes[0]);

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
                NodeServerTarget = receivedFrom;
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
                NodeServerTarget = receivedFrom;
                message.Type = ServerMessageType.SetEndianness;
                message.Target = NodeServerTarget;
                message.AddContent(BitConverter.IsLittleEndian);
                Send(message);
                Debug.LogFormat("[Server] Node component connected from {0}.", receivedFrom.ToString());
                break;
            case ServerMessageType.AppInit:
                // Assume that as join is coming over UDP app is a spectator.
                message.Target = receivedFrom;
                message.Type = ServerMessageType.AppInit;
                message.AddContent(BitConverter.IsLittleEndian);
                message.AddContent(SpheroManager.SpectatorName);
                Send(message);

                Spectator spectator = new Spectator(receivedFrom);
                if (!SpectatorManager.Instances.Contains(spectator))
                    SpectatorManager.Instances.Add(spectator);
                break;
            case ServerMessageType.VoteEvent:
                SpectatorManager.EventVote(bytes[1]);
                break;
        }
    }
}
