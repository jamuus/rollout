using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

public static class Server
{
    public enum MessageType
    {
        Test            = 0x00,
        RemoveSphero    = 0x01,
        SetEndianness   = 0x02,     // 1 -> little, 0 -> big
        UpdateState     = 0x04,
        RollSphero      = 0x08
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
            //Thread.CurrentThread.Abort();  //TODO this should be an infinite loop but it breaks unity. Need to fix.
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

    // SpheroManager state message format:
    //  + MessageType   - 1 byte
    //  + Multiple repeats of the following:
    //   + DeviceName    - 1 + n bytes
    //   + Velocity      - 8 bytes
    private static void ProcessReceivedBytes(byte[] bytes)
    {
        Debug.Log("Received from server...");
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

            Sphero sphero;
            if (!SpheroManager.Instances.TryGetValue(deviceName, out sphero))
            {
                SpheroManager.Instances.Add(deviceName, new Sphero());
                sphero = SpheroManager.Instances[deviceName];
            }

            // For now, update everything.
            sphero.DeviceName = deviceName;
            sphero.Velocity = velocity;

            string output = string.Format("DNAME: {0}, VEL: ({1},{2})", deviceName, velocity.x, velocity.y);
            Debug.Log(output);
        }
        Debug.Log("Done");
    }
}
