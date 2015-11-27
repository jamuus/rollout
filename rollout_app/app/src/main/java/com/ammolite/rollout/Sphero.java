package com.ammolite.rollout;

public final class Sphero {
    private static String deviceName;
    private static float health;
    private static float shield;
    private static float voltage;
    private static byte[] weapons;
    private static int activeWeapon;
    private static byte[] powerUps;

    static {
        deviceName = "NO STATE";
        health = 0.0f;
        shield = 0.0f;
        voltage = 0.0f;
        //weapons = null;
        weapons = new byte[1]; // single default weapon id 0.
        activeWeapon = 0;
        powerUps = null;
    }

    private Sphero() { }

    public static void roll(float direction, float force) {
        ServerMessage message = new ServerMessage(ServerMessage.Type.ROLL_SPHERO);
        message.addContent(direction);
        message.addContent(force);
        message.addContent(deviceName);
        Server.send(message);
    }

    public static void shoot(float direction) {
        ServerMessage message = new ServerMessage(ServerMessage.Type.SPHERO_SHOOT);
        message.addContent(weapons[activeWeapon]);
        message.addContent(direction);
        message.addContent(deviceName);
        Server.send(message);
    }

    public static void usePowerUp(int index) {
        ServerMessage message = new ServerMessage(ServerMessage.Type.SPHERO_POWERUP);
        message.addContent(powerUps[index]);
        message.addContent(deviceName);
        Server.send(message);
    }

    public static void pauseGame() {
        ServerMessage message = new ServerMessage(ServerMessage.Type.PAUSE_GAME);
        message.addContent(deviceName);
        Server.send(message);
    }

    public static String getDeviceName() {
        return deviceName;
    }

    public static void setDeviceName(String name) {
        deviceName = name;
    }

    public static float getHealth() {
        return health;
    }

    public static void setHealth(float health) {
        Sphero.health = health;
    }

    public static void setShield(float shield) {
        Sphero.shield = shield;
    }

    public static float getShield() {
        return shield;
    }

    public static float getBatteryVoltage() {
        return voltage;
    }

    public static void setBatteryVoltage(float voltage) {
        Sphero.voltage = voltage;
    }

    public static void setWeaponsSize(int size) {
        weapons = new byte[size];
    }

    public static int getWeaponsSize() {
        return weapons.length;
    }

    public static void setWeapon(int index, byte type) {
        weapons[index] = type;
    }

    public static byte getWeapon(int index) {
        return weapons[index];
    }

    public static void setActiveWeapon(int index) {
        activeWeapon = index;
    }

    public static int getActiveWeapon() {
        return activeWeapon;
    }

    public static void setPowerUpsSize(int size) {
        powerUps = new byte[size];
    }

    public static int getPowerUpsSize() {
        return powerUps.length;
    }

    public static void setPowerUp(int index, byte type) {
        powerUps[index] = type;
    }

    public static byte getPowerUp(int index) {
        return powerUps[index];
    }
}
