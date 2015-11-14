package com.ammolite.rollout;

import java.util.Arrays;

public final class BitConverter {
    private static boolean isLittleEndian_;

    static {
        isLittleEndian_ = true;
    }

    private BitConverter() { }

    public static void setIsLittleEndian(boolean isLittleEndian) {
        isLittleEndian_ = isLittleEndian;
    }

    public static boolean getIsLittleEndian() {
        return isLittleEndian_;
    }

    public static byte[] getBytes(float value) {
        return convertToFourBytes(Float.floatToIntBits(value));
    }

    public static byte[] getBytes(int value) {
        return convertToFourBytes(value);
    }

    public static float toFloat(byte[] bytes, int offset) {
        return Float.intBitsToFloat(convertFromFourBytes(Arrays.copyOfRange(bytes, offset, offset + 4)));
    }

    public static int toInt(byte[] bytes, int offset) {
        return convertFromFourBytes(Arrays.copyOfRange(bytes, offset, offset + 4));
    }

    private static byte[] convertToFourBytes(int bits) {
        byte[] data = new byte[4];

        data[0] = (byte)(bits & 0xff);
        data[1] = (byte)((bits >> 8) & 0xff);
        data[2] = (byte)((bits >> 16) & 0xff);
        data[3] = (byte)((bits >> 24) & 0xff);

        if (!isLittleEndian_)
            Utility.reverseArray(data);

        return data;
    }

    private static int convertFromFourBytes(byte[] bytes) {
        if (!isLittleEndian_)
            Utility.reverseArray(bytes);

        int bits = (int)(bytes[0]);
        bits |= (bytes[1] << 8);
        bits |= (bytes[2] << 16);
        bits |= (bytes[3] << 24);

        return bits;
    }
}
