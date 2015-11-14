package com.ammolite.rollout;

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
}
