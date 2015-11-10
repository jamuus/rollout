package com.ammolite.rollout;

import java.util.List;

public class Utility {
    public static List<Byte> addRange(List<Byte> list, byte[] array) {
        for (byte b : array)
            list.add(new Byte(b));
        return list;
    }
}
