package com.ammolite.rollout;

import android.os.Debug;
import android.util.Log;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InterfaceAddress;
import java.net.NetworkInterface;
import java.net.Socket;
import java.net.SocketException;
import java.util.Enumeration;

public final class Server {
    private static final String TAG             = "Server";
    private static final int    UNITY_PORT      = 7777;
    private static final int    PORT_OUTGOING   = 8888;
    private static final int    PORT_INCOMING   = 8889;
    private static final int    BUFFER_SIZE     = 2048;

    private static DatagramSocket               udpIncoming;
    private static DatagramSocket               udpOutgoing;
    private static Thread                       listeningThread;
    private static boolean                      serverListening;
    private static ServerListActivity           serverListActivity;
    private static SpectatorControllerActivity  spectatorControllerActivity;

    private static boolean              tcpServerListen;
    private static Socket               tcpSocket;
    private static DataOutputStream     toServerStream;
    private static DataInputStream      fromServerStream;
    private static Thread               tcpThread;
    private static byte[]               tcpBuffer;

    public static void startListening() {
        tcpBuffer = new byte[128];

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

        Log.d(TAG, "Server started successfully.");
    }

    public static void stopListening() {
        serverListening = false;
        tcpServerListen = false;
        udpOutgoing.close();
        udpIncoming.close();
        try {
            listeningThread.join();
            //tcpThread.join();
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

    public static void leaveServer() {
        ServerMessage message = new ServerMessage(ServerMessageType.REMOVE_SPHERO);
        message.addContent(Sphero.getName());
        sendTCP(message);

        tcpServerListen = false;
        try {
            tcpThread.join();
            tcpSocket.close();
        } catch (InterruptedException ex) {
            Log.d(TAG, "Exception stopping TCP thread.", ex);
        } catch (IOException ex) {
            Log.d(TAG, "Exception closing TCP socket.", ex);
        }

        Sphero.setName(null);
    }

    public static void leaveServerAsync() {
        new Thread(new Runnable() {
            @Override
            public void run() {
                leaveServer();
            }
        }).start();
    }

    public static boolean isTcpActive() {
        return tcpServerListen;
    }

    public static void stopTcpThread() {
        tcpServerListen = false;
        try {
            tcpSocket.close();
            tcpThread.join();
        } catch (InterruptedException ex) {
            Log.d(TAG, "Exception stopping TCP thread.", ex);
        } catch (IOException ex) {
            Log.d(TAG, "Exception closing TCP socket.", ex);
        }
    }

    public static void connectTo(ServerHandle server, boolean asPlayer) {
        ServerMessage.setDefaultTarget(server.getTarget());
        ServerMessage message = new ServerMessage(ServerMessageType.APP_INIT);
        message.addContent(asPlayer);

        if (asPlayer) {
            try {
                tcpSocket = new Socket(server.getTarget(), UNITY_PORT);
                toServerStream = new DataOutputStream(tcpSocket.getOutputStream());
                fromServerStream = new DataInputStream(tcpSocket.getInputStream());

                tcpServerListen = true;

                tcpThread = new Thread(new Runnable() {
                    @Override
                    public void run() {
                        while (tcpServerListen) {
                            try {
                                processReceivedBytesTCP(fromServerStream.read());
                            } catch (IOException ex) {
                                Log.d(TAG, "Exception receiving on TCP connection.", ex);
                            }
                        }

                        if (Sphero.getName() != null) {
                            Sphero.leaveGame();
                        }
                    }
                });
                tcpThread.start();

                sendTCP(message);
            } catch (IOException ex) {
                Log.d(TAG, "Exception opening TCP connection.", ex);
            }
        } else {
            send(message);
        }
    }

    public static void voteEvent(int eventId) {
        ServerMessage message = new ServerMessage(ServerMessageType.VOTE_EVENT);
        message.addContent((byte)(eventId & 0xff));
        sendAsync(message);
    }

    public static void sendTCP(ServerMessage message) {
        byte[] data = message.compile();
        try {
            toServerStream.write(data, 0, data.length);
        } catch (IOException ex) {
            Log.d(TAG, "Exception sending TCP message.", ex);
        }
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

    public static void sendTCPAsync(final ServerMessage message) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                sendTCP(message);
            }
        }).start();
    }

    public static void sendAsync(final ServerMessage message) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                send(message);
            }
        }).start();
    }

    private static void readStreamedBytes(int count, int offset) {
        int remaining = count;
        while (remaining > 0) {
            try {
                int read = fromServerStream.read(tcpBuffer, offset, remaining);
                remaining -= read;
                offset += read;
            } catch (IOException ex) {
                Log.d(TAG, "Failed reading to TCP buffer.", ex);
            }
        }
    }

    private static void readStreamedBytes(int count) {
        readStreamedBytes(count, 0);
    }

    private static void processReceivedBytesTCP(int type) {
        switch (type) {
            case ServerMessageType.APP_INIT:
                readStreamedBytes(2);
                BitConverter.setIsLittleEndian(BitConverter.toBoolean(tcpBuffer, 0));
                int nameLength = tcpBuffer[1];
                readStreamedBytes(nameLength);
                serverListActivity.joinServerAs(Utility.extractString(tcpBuffer, 0, nameLength));
                break;
            case ServerMessageType.RESTART:
                Sphero.restart();
                break;
            case ServerMessageType.UPDATE_STATE:
                readStreamedBytes(13);
                readStreamedBytes(tcpBuffer[12] + 1, 13);
                readStreamedBytes(tcpBuffer[13 + tcpBuffer[12]], 14 + tcpBuffer[12]);
                Sphero.parseState(tcpBuffer);
                break;
            default:
                break;
        }
    }

    private static void processReceivedBytes(final byte[] bytes, InetAddress receivedFrom) {
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
            case ServerMessageType.SET_EVENTS:
                spectatorControllerActivity.runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        spectatorControllerActivity.setEvents(bytes[1], bytes[2]);
                        spectatorControllerActivity.startCountdown(BitConverter.toInt(bytes, 3));
                    }
                });
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

    public static void setSpectatorControllerActivity(SpectatorControllerActivity activity) {
        spectatorControllerActivity = activity;
    }
}