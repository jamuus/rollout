package com.ammolite.rollout;
import android.util.Log;

import java.io.UnsupportedEncodingException;
import java.util.Arrays;
import java.util.List;

public final class Utility {
    private static final String TAG = "Utility";

    private Utility() { }

    public static byte[] listToUnboxedArray(List<Byte> in, int padFront, int padBack) {
        byte[] data = new byte[in.size() + padFront + padBack];
        for (int i = 0; i < in.size(); ++i)
            data[i + padFront] = in.get(i);
        return data;
    }

    public static List<Byte> addUnboxedArray(List<Byte> out, byte[] data) {
        for (int i = 0; i < data.length; ++i)
            out.add(data[i]);
        return out;
    }

    public static byte[] reverse(byte[] data) {
        for (int i = 0; i < data.length / 2; ++i) {
            int idx = data.length - i - 1;
            byte tmp = data[i];
            data[i] = data[idx];
            data[idx] = tmp;
        }
        return data;
    }

    public static byte[] extractSubset(byte[] data, int start, int size) {
        return Arrays.copyOfRange(data, start, start + size);
    }

    public static String extractString(byte[] data, int start, int length) {
        try {
            return new String(extractSubset(data, start, length), "US-ASCII");
        } catch (UnsupportedEncodingException ex) {
            Log.d(TAG, "Bad encoding.", ex);
            return null;
        }
    }

    public static void asyncTask(Runnable task) {
        new Thread(task).start();
    }
}