package com.ammolite.rollout;

import java.util.List;

public final class Utility {
    private Utility() { }

    public static List<Byte> addRange(List<Byte> list, byte[] array) {
        for (byte b : array)
            list.add(b);
        return list;
    }

    public static byte[] reverseArray(byte[] array) {
        for (int i = 0; i < array.length / 2; ++i) {
            final int idx = array.length - i - 1;
            byte tmp = array[i];
            array[i] = array[idx];
            array[idx] = tmp;
        }
        return array;
    }
}
