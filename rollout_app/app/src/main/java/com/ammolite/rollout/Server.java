package com.ammolite.rollout;

import android.util.Log;

import java.io.UnsupportedEncodingException;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;

public final class Server {
    public static class MessageType {
        public static final int TEST            = 0x00;
        public static final int REMOVE_SPHERO   = 0x01;
        public static final int SET_ENDIANNESS  = 0x02;
        public static final int UPDATE_STATE    = 0x04;
        public static final int ROLL_SPHERO     = 0x08;
    }

    public class Message {
        private static final String TAG = "SERVER.MESSAGE";

        private int         type_;
        private List<Byte>  data_;

        public Message() {
            this.data_ = new ArrayList<>();
        }

        public Message(int type) {
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

        }
    }

    private static final String IP_ADDRESS      = "127.0.0.1";
    private static final int    PORT            = 7777;

    private Server() { }
}
