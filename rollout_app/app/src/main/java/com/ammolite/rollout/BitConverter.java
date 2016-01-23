package com.ammolite.rollout;

import java.util.Arrays;

public final class BitConverter {
    private BitConverter() { }

    private static boolean isLittleEndian = true;

    public static boolean getIsLittleEndian() {
        return isLittleEndian;
    }

    public static void setIsLittleEndian(boolean value) {
        isLittleEndian = value;
    }

    public static byte[] getBytes(float value) {
        return convert(Float.floatToIntBits(value), 4);
    }

    public static byte[] getBytes(int value) {
        return convert(value, 4);
    }

    public static float toFloat(byte[] bytes, int offset) {
        return Float.intBitsToFloat(convertBack(bytes, offset, 4));
    }

    public static int toInt(byte[] bytes, int offset) {
        return convertBack(bytes, offset, 4);
    }

    public static boolean toBoolean(byte[] bytes, int position) {
        return (bytes[position] == 1);
    }

    private static byte[] convert(int bits, int size) {
        byte[] bytes = new byte[size];
        for (int i = 0; i < size; ++i)
            bytes[i] = (byte)((bits >> (i * 8)) & 0xff);
        return isLittleEndian ? bytes : Utility.reverse(bytes);
    }

    private static int convertBack(byte[] bytes, int offset, int size) {
        int bits = 0;
        if (!isLittleEndian) {
            bytes = Arrays.copyOfRange(bytes, offset, offset + size);
            Utility.reverse(bytes);
            for (int i = 0; i < size; ++i)
                bits |= (((int)bytes[i]) << (i * 8)); // TODO Big endian has not been tested.
        } else {
            for (int i = 0; i < size; ++i) {
                bits |= ((int)bytes[offset + i] & 0xff) << (i * 8);
            }
        }
        return bits;
    }
}