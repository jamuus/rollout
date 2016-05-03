using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;

public class TcpServerModule
{
    public class Connection
    {
        public Connection(Socket socket)
        {
            this.Socket = socket;

            Buffer = new byte[128];
            BufferOffset = 0;
            BufferRemaining = 1;
        }

        public Socket Socket          { get; set; }
        public byte[] Buffer          { get; set; }
        public int    BufferOffset    { get; set; }
        public int    BufferRemaining { get; set; }
    }


    #region Private Members

    private Socket listenSocket;

    #endregion


    #region Properties

    public int              Port        { get; set;         }
    public List<Connection> Connections { get; private set; }

    #endregion


    #region Event Handlers

    // Connection established.
    public event EventHandler<SocketAsyncEventArgs> Connected;

    // Connection terminated.
    public event EventHandler<SocketAsyncEventArgs> Disconnected;

    // Data received on the socket.
    public event EventHandler<SocketAsyncEventArgs> DataReceived;

    // Data sent on the socket.
    public event EventHandler<SocketAsyncEventArgs> DataSent;

    #endregion


    #region Constructors

    public TcpServerModule() : this(7777) { }

    public TcpServerModule(int port)
    {
        Connections = new List<Connection>();
        Port = port;
    }

    #endregion


    #region Start and Stop

    public void Start()
    {
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, Port);

        listenSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listenSocket.Bind(endpoint);
        listenSocket.Listen(128);

        Log("Started listening on port {0}.", Port);

        Accept(null);
    }

    public void Stop()
    {
        foreach (Connection connection in Connections)
            Disconnect(connection);

        Connections.Clear();
        listenSocket.Close();

        Log("Stopped listening and closed all active connections.");
    }

    #endregion


    #region Accept Client Connections

    // Request for accepting a new connection.
    private void Accept(SocketAsyncEventArgs args)
    {
        Log("Awaiting incoming connection.");

        // If null, need to create a new object. This should
        // only happen when called from TcpServerModule.Start().
        if (args == null)
        {
            // TODO Store in pool instead to prevent fragmentation.
            args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompelted;
        }
        // Otherwise, reuse and reset args.
        else
        {
            args.AcceptSocket = null;
        }

        // Accept an incoming connection. False indicates synchronous compeletion.
        if (listenSocket.AcceptAsync(args) == false)
            OnAcceptCompelted(this, args);
    }

    // Callback for when an accept operation has been compelted successfully. Verifies
    // the accept occurred correctly and setup a stream.
    private void OnAcceptCompelted(object sender, SocketAsyncEventArgs args)
    {
        Log("Accepted incoming connection.");

        // Set socket options.
        args.AcceptSocket.NoDelay = true;

        // Create connection for the socket.
        Connection connection = new Connection(args.AcceptSocket);
        Connections.Add(connection);

        // Perform the first receive operation.
        SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
        receiveArgs.UserToken = connection;

        // Perform connection events if there are any.
        if (Connected != null)
            Connected(this, receiveArgs);

        receiveArgs.Completed += OnReceiveCompleted;
        Receive(receiveArgs);

        // Await for more connections.
        Accept(args);
    }

    #endregion


    #region Receiving Data

    // Starts an asynchronous receive.
    private void Receive(SocketAsyncEventArgs args)
    {
        // Get the relevant connection.
        Connection connection = args.UserToken as Connection;

        if (connection != null)
        {
            args.SetBuffer(connection.Buffer, connection.BufferOffset, connection.BufferRemaining);
            if (connection.Socket.ReceiveAsync(args) == false)
                OnReceiveCompleted(this, args);
        }
    }

    // Fires when a receive has been compelted. First checks if the receive was
    // successful and the connection is still valid, then notifies any lisenters.
    private void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
    {
        Connection connection = args.UserToken as Connection;

        // If the connection is invalid, an error occurred, or no data was received, the connection is
        // no longer valid and is disconnected.
        if (connection == null || args.SocketError != SocketError.Success || args.BytesTransferred == 0)
        {
            Log("Receive failed. Connection: {0}, SocketError: {1}, BytesTransferred: {2}.", connection != null, args.SocketError, args.BytesTransferred);
            Disconnect(args);
            return;
        }

        // Notify listeners if the message is compelte.
        if (DataReceived != null && EnsureDataComplete(args))
            DataReceived(this, args);

        // Wait for the next receive.
        Receive(args);
    }

    // Handle the type of data received and make sure the whole message is present before attempting
    // to parse it.
    private bool EnsureDataComplete(SocketAsyncEventArgs args)
    {
        Connection connection = args.UserToken as Connection;

        ServerMessageType type = (ServerMessageType)connection.Buffer[0];
        int length = ServerMessage.Length(type);

        connection.BufferOffset += args.BytesTransferred;
        connection.BufferRemaining = length - connection.BufferOffset;

        if (type == ServerMessageType.RollSphero && connection.BufferOffset >= length)
            connection.BufferRemaining += connection.Buffer[9];
        else if (type == ServerMessageType.SpheroShoot && connection.BufferOffset >= length)
            connection.BufferRemaining += connection.Buffer[6];
        else if (type == ServerMessageType.SpheroPowerUp && connection.BufferOffset >= length)
            connection.BufferRemaining += connection.Buffer[2];
        else if (type == ServerMessageType.RemoveSphero && connection.BufferOffset >= length)
            connection.BufferRemaining += connection.Buffer[1];

        return connection.BufferRemaining <= 0;
    }

    #endregion


    #region Sending

    // Send a ServerMessage.
    public bool Send(Connection connection, ServerMessage message)
    {
        byte[] data = message.Compile();
        return Send(connection, data, 0, data.Length);
    }

    // Send on a given connection asynchronously.
    public bool Send(Connection connection, byte[] data, int offset, int length)
    {
        if (connection.Socket == null || connection.Socket.Connected == false)
        {
            Log("Failed to send data on connection.");
            Disconnect(connection);
            return false;
        }

        // Create send event args.
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        sendArgs.UserToken = connection;
        sendArgs.Completed += OnSendCompelte;
        sendArgs.SetBuffer(data, offset, length);

        try
        {
            if (connection.Socket.SendAsync(sendArgs) == false)
                OnSendCompelte(this, sendArgs);
        }
        catch (SocketException ex)
        {
            Log("SocketException occurred when sending, code {0}.", ex.ErrorCode);
            return false;
        }
        catch (ObjectDisposedException)
        {
            Log("Attempted to send data on a closed socket.");
            return false;
        }

        // Notify listeners.
        if (DataSent != null)
            DataSent(this, sendArgs);

        return true;
    }

    // Fires when an asynchronous send operation has been completed.
    private void OnSendCompelte(object sender, SocketAsyncEventArgs args)
    {
        args.Completed -= OnSendCompelte;
    }

    #endregion


    #region Disconnecting

    // Disconnect on the event a send or receive fails. Gracefully closes the
    // connection.
    private void Disconnect(SocketAsyncEventArgs args)
    {
        Connection connection = args.UserToken as Connection;

        if (connection == null)
        {
            Log("No valid connection when disconnecting.");
            // TODO Handle this somehow. Should never happen.
        }

        try
        {
            // Close connection for both sending and receiving.
            connection.Socket.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException ex)
        {
            Log("SocketException occurred when disconnecting: code {0}.", ex.ErrorCode);
        }
        catch (ObjectDisposedException)
        {
            Log("Socket already closed before disconnecting.");
        }
        finally
        {
            // Close the socket and remove the connection from the active connections list.
            connection.Socket.Close();
            Connections.Remove(connection);

            // Fire any listeners.
            if (Disconnected != null)
                Disconnected(this, args);

            args.Completed -= OnReceiveCompleted;

            Log("Disconnected client socket.");
        }
    }

    // Close a connection gracefully from another part of the system. Works by forcing the
    // next send or receive to fail, causing Disconnect(SocketAsyncEventArgs) to be called.
    public void Disconnect(Connection connection)
    {
        try
        {
            connection.Socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {
        }
        finally
        {
            Connections.Remove(connection);

            foreach (KeyValuePair<string, Sphero> s in SpheroManager.Instances)
            {
                if (s.Value.Connection == connection)
                {
                    s.Value.Leave();
                    break;
                }
            }

            Log("Disconnected client socket.");
        }
    }

    #endregion


    #region Logging

    private void Log(string format, params object[] args)
    {
        Debug.LogFormat("[TcpServerModule@{0}|{1}] " + string.Format(format, args), Thread.CurrentThread.ManagedThreadId,(int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
    }

    #endregion
}
