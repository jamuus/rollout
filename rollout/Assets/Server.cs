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
	NodeInit		= 0x11,
	AppInit			= 0x21
}

public class ServerMessage
{
	public ServerMessageType    Type   { get; set; }
	public IPEndPoint           Target { get; set; }
	
	private List<byte> data;
	
	public ServerMessage()
	{
		data = new List<byte>();
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
		byte[] bytes = new byte[data.Count + 1];
		bytes[0] = (byte)Type;
		Buffer.BlockCopy(data.ToArray(), 0, bytes, 1, data.Count);
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
		
		//Console.WriteLine("[Server] Started \"{0}\" successfully, listening on port {1}.",
		//                  Name, port);
		Debug.LogFormat("[Server] Started \"{0}\" successfully, listening on port {1}.",
		                Name, port);
	}
	
	public static void StopListening()
	{
		listenThread.Abort();
		//Console.WriteLine("[Server] Stopped successfully.");
		Debug.LogFormat("[Server] Stopped successfully.");
	}
	
	public static void Send(ServerMessage message)
	{
		byte[] bytes = message.Compile();
		udpOutgoing.Send(bytes, bytes.Length, message.Target);
	}
	
	private static void ProcessReceivedBytes(byte[] bytes, IPEndPoint from)
	{
		string prefix = string.Format("[Server] {0} - ", from.ToString());
		
		if (!Enum.IsDefined(typeof(ServerMessageType), (int)bytes[0]))
		{
			//Console.WriteLine("{0} Unknown (0x{1:x2}).", prefix, bytes[0]);
			Debug.LogFormat("{0} Unknown (0x{1:x2}).", prefix, bytes[0]);
			return;
		}
		
		ServerMessage message = new ServerMessage();
		ServerMessageType type = (ServerMessageType)bytes[0];
		
		//Console.WriteLine("{0} {1} (0x{2:x2}).", prefix, type.ToString(), bytes[0]);
		Debug.LogFormat("{0} {1} (0x{2:x2}).", prefix, type.ToString(), bytes[0]);
		
		switch (type)
		{
		case ServerMessageType.Test:
			break;
		case ServerMessageType.RemoveSphero:
			break;
		case ServerMessageType.SetEndianness:
			message.Type = ServerMessageType.SetEndianness;
			message.Target = from;
			message.AddContent(BitConverter.IsLittleEndian);
			Send(message);
			break;
		case ServerMessageType.UpdateState:
			break;
		case ServerMessageType.RollSphero:
			// For now, forward the message to unity.
			message.Type = ServerMessageType.RollSphero;
			message.Target = nodeServerTarget;
			for (int i = 1; i < bytes.Length; ++i)
				message.AddContent(bytes[i]);
			Send(message);
			break;
		case ServerMessageType.ServerDiscover:
			message.Type = ServerMessageType.ServerDiscover;
			message.Target = from;
			message.AddContent(Name);
			Send(message);
			break;
		case ServerMessageType.SpheroShoot:
			break;
		case ServerMessageType.SpheroPowerUp:
			break;
		case ServerMessageType.PauseGame:
			//TODO pause unity update (timestep=0) [send PauseGame to all controllers].
			break;
		case ServerMessageType.NodeInit:
			// When node identifies itself, send the endianness.
			nodeServerTarget = from;
			message.Type = ServerMessageType.SetEndianness;
			message.Target = nodeServerTarget;
			message.AddContent(BitConverter.IsLittleEndian);
			Send(message);
			break;
		case ServerMessageType.AppInit:
			//TODO apps' IPEndPoint need to be stored wrt their sphero, for now just send a junk name.
			message.Type = ServerMessageType.AppInit;
			message.Target = from;
			message.AddContent(BitConverter.IsLittleEndian);
			message.AddContent("Sphero-BOO");
			Send(message);
            // For testing
            SpheroManager.Instances["SPHERO-BOO"].ControllerTarget = from;
            SpheroManager.Instances["SPHERO-BOO"].SendStateToController();
			break;
		}
	}
}

/*
public static class Server
{
    public enum MessageType 
	{
        Test            = 0x00,
        RemoveSphero    = 0x01,
        SetEndianness   = 0x02,     // 1 -> little, 0 -> big
        UpdateState     = 0x04,
        RollSphero      = 0x08,
		ServerDiscover	= 0x10,
		SpheroShoot		= 0x20,
		SpheroPowerUp	= 0x40,
		PauseGame		= 0x80
    }

    public class Message
    {
        public MessageType Type { get; set; }

        private List<byte> data_;

        public Message()
        {
            data_ = new List<byte>();
        }

        public Message(MessageType type) :
			this()
        {
            Type = type;
        }

        // Add content methods append bytes to the message content.
        // For a string, the first byte is the length.
        // Null terminators are not encoded.
        public void AddContent(string content)
        {
            data_.Add((byte)content.Length);
            data_.AddRange(Encoding.ASCII.GetBytes(content));
        }

        public void AddContent(Vector2 content)
        {
            data_.AddRange(BitConverter.GetBytes(content.x));
            data_.AddRange(BitConverter.GetBytes(content.y));
        }

        public void AddContent(byte content)
        {
            data_.Add(content);
        }

        public void AddContent(bool content)
        {
            data_.Add(Convert.ToByte(content));
        }

        public void AddContent(float content)
        {
            data_.AddRange(BitConverter.GetBytes(content));
        }

        public void AddContent(double content)
        {
            data_.AddRange(BitConverter.GetBytes(content));
        }

        public void AddContent(int content)
        {
            data_.AddRange(BitConverter.GetBytes(content));
        }

        // Create byte array representing message.
        public byte[] Compile()
        {
            byte[] bytes = new byte[data_.Count + 1];
            bytes[0] = (byte)Type;
            Buffer.BlockCopy(data_.ToArray(), 0, bytes, 1, data_.Count);
            return bytes;
        }
    }

    private static UdpClient connection_;
    private static IPEndPoint ip_;
    private static Thread listener_;

    public static void OpenConnection(string ip, int port)
    {
        connection_ = new UdpClient(7778);
        ip_ = new IPEndPoint(IPAddress.Parse(ip), port);

        // Create a sepratate thread to listen for any incoming states
        // from server.
        listener_ = new Thread(() => 
		{
            Thread.CurrentThread.IsBackground = true;

            UdpClient localConnection = new UdpClient(7779);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);

            while (true)
                ProcessReceivedBytes(localConnection.Receive(ref ipe));
        });
        listener_.Start();
    }

    public static void CloseConnection()
    {
        listener_.Abort();
    }

    public static void Send(Message message)
    {
        byte[] bytes = message.Compile();
        connection_.Send(bytes, bytes.Length, ip_);
    }

    public static void SendEndianness()
    {
        byte[] bytes = new byte[] { (byte)MessageType.SetEndianness,
			Convert.ToByte(BitConverter.IsLittleEndian) };
        connection_.Send(bytes, bytes.Length, ip_);
    }

	private static void ProcessReceivedBytes(byte[] bytes)
	{
		MessageType type = (MessageType)bytes[0];

		switch (type) 
		{
		case MessageType.ServerDiscover:
			break;
		case MessageType.RollSphero:
			break;
		case MessageType.PauseGame:
			break;
		case MessageType.SpheroShoot:
			break;
		case MessageType.SpheroPowerUp:
			break;
		default:
			Debug.Log(String.Format ("Unknown message type: {0:x2}.", bytes[0]));
			break;
		}
	}

    // SpheroManager state message format:
    //  + MessageType   - 1 byte
    //  + Multiple repeats of the following:
    //   + DeviceName    - 1 + n bytes
    //   + Velocity      - 8 bytes
	/*
    private static void ProcessReceivedBytes(byte[] bytes)
    {
        MessageType type = (MessageType)bytes[0];

        if (type != MessageType.UpdateState)
		{
            Debug.Log(string.Format("Unexpected message type received: {0:x2}", bytes[0]));
            return;
        }

        // If no data, stop.
        if (bytes.Length == 1)
            return;

        // At the moment, doesn't support removing of spheros.
        // Also no error checking.
        int index = 1;
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
            position.x = BitConverter.ToSingle(bytes, index);
            index += 4;
            position.y = BitConverter.ToSingle(bytes, index);
            index += 4;

            float voltage = BitConverter.ToSingle(bytes, index);
            index += 4;

            Sphero sphero;
            if (!SpheroManager.Instances.TryGetValue(deviceName, out sphero)) 
			{
                SpheroManager.Instances.Add(deviceName, new Sphero());
                sphero = SpheroManager.Instances[deviceName];
            }

            // For now, update everything.
            sphero.DeviceName = deviceName;
            sphero.Velocity = velocity;
            sphero.Position = position;
            sphero.BatteryVoltage = voltage;
        }
    }
}
*/