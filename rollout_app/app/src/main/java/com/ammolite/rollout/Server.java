package com.ammolite.rollout;

import android.util.Log;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InterfaceAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.Enumeration;

public final class Server {
    private static final String TAG             = "Server";
    private static final int    UNITY_PORT      = 7777;
    private static final int    PORT_OUTGOING   = 8888;
    private static final int    PORT_INCOMING   = 8889;
    private static final int    BUFFER_SIZE     = 2048;

    private static DatagramSocket       udpIncoming;
    private static DatagramSocket       udpOutgoing;
    private static Thread               listeningThread;
    private static boolean              serverListening;
    private static ServerListActivity   serverListActivity;

    public static void startListening() {
        try {
            udpIncoming = new DatagramSocket(PORT_INCOMING);
            udpOutgoing = new DatagramSocket(PORT_OUTGOING);
            udpOutgoing.setBroadcast(true);
        } catch (SocketException ex) {
            Log.e(TAG, "Failed to open UDP connection.", ex);
            return;
        }

        listeningThread = new Thread(new Runnable() {
            @Override
            public void run() {
                byte[] receivedData = new byte[BUFFER_SIZE];
                DatagramPacket packet = new DatagramPacket(receivedData, receivedData.length);

                serverListening = true;
                while (serverListening) {
                    try {
                        udpIncoming.receive(packet);
                        processReceivedBytes(receivedData, packet.getAddress());
                    } catch (IOException ex) {
                        Log.d(TAG, "Exception when receiving data on listening thread.", ex);
                    }
                }
            }
        });
        listeningThread.start();
    }

    public static void stopListening() {
        serverListening = false;
        udpOutgoing.close();
        udpIncoming.close();
        try {
            listeningThread.join();
        } catch (InterruptedException ex) {
            Log.d(TAG, "Listening thread interrupted while joining.", ex);
        }
    }

    public static void discoverServers(ServerListActivity activity) {
        serverListActivity = activity;

        byte[] messageBuffer = new byte[] { (byte)ServerMessageType.SERVER_DISCOVER };

        // Broadcast over all network interfaces.
        try {
            Enumeration<NetworkInterface> networkInterfaces = NetworkInterface.getNetworkInterfaces();
            while (networkInterfaces.hasMoreElements()) {
                NetworkInterface networkInterface = networkInterfaces.nextElement();

                if (networkInterface.isLoopback() || !networkInterface.isUp())
                    continue;

                for (InterfaceAddress address : networkInterface.getInterfaceAddresses()) {
                    InetAddress broadcast = address.getBroadcast();
                    if (broadcast == null)
                        continue;

                    try {
                        DatagramPacket packet = new DatagramPacket(messageBuffer, messageBuffer.length, broadcast, UNITY_PORT);
                        udpOutgoing.send(packet);
                    } catch (IOException ex) {
                        Log.d(TAG, "Exception when sending broadcast to " + broadcast.getHostAddress(), ex);
                    }

                    Log.d(TAG, "Server discover: " + broadcast.getHostAddress());
                }
            }
        } catch (SocketException ex) {
            Log.e(TAG, "Socket exception in server discovery.", ex);
        }
    }

    public static void connectTo(ServerHandle server, boolean asPlayer) {
        ServerMessage.setDefaultTarget(server.getTarget());
        ServerMessage message = new ServerMessage(ServerMessageType.APP_INIT);
        message.addContent(asPlayer);
        send(message);
    }

    public static void send(ServerMessage message) {
        byte[] data = message.compile();
        try {
            DatagramPacket packet = new DatagramPacket(data, data.length, message.getTarget(), UNITY_PORT);
            udpOutgoing.send(packet);
        } catch (IOException ex) {
            Log.d(TAG, "Exception sending message.", ex);
        }
    }

    public static void sendAsync(final ServerMessage message) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                send(message);
            }
        }).start();
    }

    private static void processReceivedBytes(byte[] bytes, InetAddress receivedFrom) {
        if (bytes.length <= 1) {
            Log.d(TAG, "Received invalid message (too short).");
            return;
        }

        int type = bytes[0];
        switch (type) {
            case ServerMessageType.SERVER_DISCOVER:
                serverListActivity.addServerHandle(new ServerHandle(receivedFrom, Utility.extractString(bytes, 2, bytes[1])));
                break;
            case ServerMessageType.APP_INIT:
                BitConverter.setIsLittleEndian(BitConverter.toBoolean(bytes, 1));
                serverListActivity.joinServerAs(Utility.extractString(bytes, 3, bytes[2]));
                break;
            case ServerMessageType.UPDATE_STATE:
                Sphero.parseState(bytes);
                break;
            default:
                Log.d(TAG, "Unknown message type \"" + type + "\".");
                break;
        }
    }

    public static void discoverServersAsync(final ServerListActivity activity) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                discoverServers(activity);
            }
        }).start();
    }

    public static void connectToAsync(final ServerHandle server, final boolean asPlayer) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                connectTo(server, asPlayer);
            }
        }).start();
    }
}