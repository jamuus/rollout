package com.ammolite.rollout;

import android.util.Log;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;

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

    public static void send(ServerMessage message) {
        byte[] bytes = message.Compile();
        try {
            connection_.send(new DatagramPacket(bytes, bytes.length, ip_));
        } catch (IOException ex) {
            Log.d(TAG, "Encountered a socket exception sending data to the server.", ex);
        }
    }
}
