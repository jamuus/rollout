package com.ammolite.rollout;

import android.util.Log;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.net.SocketException;

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
}
