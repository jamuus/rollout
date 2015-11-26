package com.ammolite.rollout;

import android.util.Log;

import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.List;

public class ServerMessage {
    public static final class Type {
        public static final int TEST            = 0x00;
        public static final int REMOVE_SPHERO   = 0x01;
        public static final int SET_ENDIANNESS  = 0x02;
        public static final int UPDATE_STATE    = 0x04;
        public static final int ROLL_SPHERO     = 0x08;
        public static final int SERVER_DISCOVER = 0x10;
        public static final int SPHERO_SHOOT    = 0x20;
        public static final int SPHERO_POWERUP  = 0x40;
        public static final int PAUSE_GAME      = 0x80;
    }

    private static final String TAG = "SERVER_MESSAGE";

    private int         type_;
    private List<Byte>  data_;

    public ServerMessage() {
        this.data_ = new ArrayList<>();
    }

    public ServerMessage(int type) {
        this.type_ = type;
        this.data_ = new ArrayList<>();
    }

    public void setType(int type) {
        this.type_ = type;
    }

    public int getType() {
        return type_;
    }

    public void addContent(String content) {
        data_.add((byte)content.length());
        try {
            Utility.addRange(data_, content.getBytes("US-ASCII"));
        } catch (UnsupportedEncodingException ex) {
            Log.d(TAG, "Exception occurred trying to get bytes from string.", ex);
        }
    }

    public void addContent(byte content) {
        data_.add(content);
    }

    public void addContent(boolean content) {
        data_.add((byte)(content ? 1 : 0));
    }

    public void addContent(float content) {
        Utility.addRange(data_, BitConverter.getBytes(content));
    }

    public void addContent(int content) {
        Utility.addRange(data_, BitConverter.getBytes(content));
    }

    public byte[] Compile() {
        byte[] bytes = new byte[data_.size() + 1];
        bytes[0] = (byte)(type_ & 0xff);
        for (int i = 0; i < data_.size(); ++i)
            bytes[i + 1] = data_.get(i);
        return bytes;
    }
}