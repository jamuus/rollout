package com.ammolite.rollout;

import android.util.Log;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.InterfaceAddress;
import java.net.NetworkInterface;
import java.net.SocketAddress;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

public final class Server {
    private static final String TAG = "SERVER";
    private static DatagramSocket connection_;
    private static SocketAddress ip_;

    private Server() { }

    public static void openConnection(String ip, int port) {
        try {
            connection_ = new DatagramSocket(7778);
            ip_ = new InetSocketAddress(ip, port);
        } catch (SocketException ex) {
            Log.d(TAG, "Encountered a socket exception when opening server connection.", ex);
        }
    }

    public static void closeConnection() {
        if (connection_ != null)
            connection_.close();
    }

    public static void sendSync(ServerMessage message) {
        byte[] bytes = message.Compile();
        try {
            connection_.send(new DatagramPacket(bytes, bytes.length, ip_));
        } catch (IOException ex) {
            Log.d(TAG, "Encountered a socket exception sending data to the server.", ex);
        }
    }

    public static void batchSendSync(ServerMessage[] messages) {
        for (ServerMessage m : messages)
            sendSync(m);
    }

    public static List<Pair<String, Integer>> discoverServersSync() {
        List<Pair<String, Integer>> servers = new ArrayList<>();

        byte[] data = new byte[] { (byte)ServerMessage.Type.SERVER_DISCOVER };

        DatagramSocket socket = null;

        try {
            socket = new DatagramSocket();
        } catch (SocketException ex) {
            Log.d(TAG, "Exception occurred creating socket for server discovery.", ex);
            return servers;
        }

        try {
            DatagramPacket discoverMessage = new DatagramPacket(data, data.length,
                    InetAddress.getByName("255.255.255.255"), 7777);
            socket.send(discoverMessage);
        } catch (UnknownHostException ex) {
            Log.d(TAG, "Unable to find specified host.", ex);
        } catch (IOException ex) {
            Log.d(TAG, "Unable to send packet.", ex);
        }

        // Broadcast over all network interfaces.
        try {
            Enumeration netInterfaces = NetworkInterface.getNetworkInterfaces();
            while (netInterfaces.hasMoreElements()) {
                NetworkInterface ni = (NetworkInterface)netInterfaces.nextElement();
                if (ni.isLoopback() || !ni.isUp())
                    continue;
                for (InterfaceAddress addr : ni.getInterfaceAddresses()) {
                    InetAddress bc = addr.getBroadcast();
                    if (bc == null)
                        continue;
                    Log.d(TAG, "inet-broadcast: " + bc.getHostAddress() + ".");
                    try {
                        DatagramPacket packet = new DatagramPacket(data, data.length,
                                    bc, 7777);
                        socket.send(packet);
                    } catch (IOException ex) {
                        Log.d(TAG, "Unable to send discovery packet.", ex);
                    }
                }
            }
        } catch (SocketException ex) {
            Log.d(TAG, "Failed broadcasting to network interfaces.", ex);
        }

        // Wait for response.
        try {
            byte[] recv = new byte[1];
            DatagramPacket response = new DatagramPacket(recv, recv.length);
            socket.setSoTimeout(10000);
            socket.receive(response);

            Log.d(TAG, "Got response from server " + response.getAddress().getHostAddress() + ".");
            if (recv[0] == ServerMessage.Type.SERVER_DISCOVER)
                servers.add(new Pair<>(
                        response.getAddress().getHostAddress(),
                        response.getPort()
                ));
        } catch (SocketTimeoutException ex) {
            socket.close();
            return null;
        } catch (IOException ex) {
            Log.d(TAG, "Failed while getting discovery response.", ex);
        } finally {
            socket.close();
        }
        return servers;
    }

    public static void send(final ServerMessage message) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                sendSync(message);
            }
        }).start();
    }

    public static void batchSend(final ServerMessage[] messages) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                batchSendSync(messages);
            }
        }).start();
    }

    public static Future<List<Pair<String, Integer>>> discoverServers() {
        ExecutorService executor = Executors.newSingleThreadExecutor();
        return executor.submit(new Callable<List<Pair<String, Integer>>>() {
            @Override
            public List<Pair<String, Integer>> call() throws Exception {
                return discoverServersSync();
            }
        });
    }
}
