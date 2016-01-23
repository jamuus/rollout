package com.ammolite.rollout;

import android.util.Log;

import java.io.UnsupportedEncodingException;
import java.net.InetAddress;
import java.util.ArrayList;
import java.util.List;

public class ServerMessage {
    private static final String TAG = "ServerMessage";

    private static InetAddress defaultTarget = null;

    private int         type;
    private InetAddress target;
    private List<Byte>  data;

    public ServerMessage() {
        this(ServerMessageType.TEST);
    }

    public ServerMessage(int type) {
        data = new ArrayList<Byte>();
        this.type = type;
        target = defaultTarget;
    }

    public int getType() {
        return type;
    }

    public InetAddress getTarget() {
        return target;
    }

    public void setType(int type) {
        this.type = type;
    }

    public void setTarget(InetAddress target) {
        this.target = target;
    }

    public void addContent(String content) {
        try {
            data.add((byte)content.length());
            data = Utility.addUnboxedArray(data, content.getBytes("US-ASCII"));
        } catch (UnsupportedEncodingException ex) {
            Log.d(TAG, "Unsupported encoding adding content.", ex);
        }
    }

    public void addContent(byte content) {
        data.add(content);
    }

    public void addContent(byte[] content) {
        data = Utility.addUnboxedArray(data, content);
    }

    public void addContent(boolean content) {
        data.add((byte)(content ? 1 : 0));
    }

    public void addContent(int content) {
        data = Utility.addUnboxedArray(data, BitConverter.getBytes(content));
    }

    public void addContent(float content) {
        data = Utility.addUnboxedArray(data, BitConverter.getBytes(content));
    }

    public byte[] compile() {
        byte[] bytes = Utility.listToUnboxedArray(data, 1, 0);
        bytes[0] = (byte)type;
        return bytes;
    }

    public static void setDefaultTarget(InetAddress target) {
        defaultTarget = target;
    }
}