package com.ammolite.rollout;

import android.util.Log;

import java.util.concurrent.Callable;

public final class Sphero {
    private static final String TAG                 = "Sphero";
    public  static final int    UPDATE_MS_PER_TICK  = 16;           // ~ 64 ticks/s

    private static String       name;
    private static float        voltage;
    private static float        health;
    private static float        maxHealth;
    private static float        shield;
    private static int          ticksBetweenShots = 100;
    private static int          ticksSinceLastShot;
    private static byte[]       weapons;
    private static byte[]       powerUps;
    private static int          activeWeapon;
    private static Thread       updateThread;
    private static boolean      updateThreadIsRunning;
    private static boolean      recentDamage;

    private Sphero() { }

    public static void parseState(byte[] bytes) {
        float oldHealth = health;
        float oldShiled = shield;

        int offset = 1;
        health = BitConverter.toFloat(bytes, offset);
        if (maxHealth == 0) maxHealth = health;
        offset += 4;
        shield = BitConverter.toFloat(bytes, offset);
        offset += 4;
        voltage = BitConverter.toFloat(bytes, offset);
        offset += 4;

        weapons = new byte[bytes[offset]];
        ++offset;
        System.arraycopy(bytes, offset, weapons, 0, weapons.length);
        offset += weapons.length;

        powerUps = new byte[bytes[offset]];
        ++offset;
        System.arraycopy(bytes, offset, powerUps, 0, powerUps.length);
        offset += powerUps.length;

        activeWeapon = 0;

        if ((oldHealth > health) || (oldShiled > shield))
            recentDamage = true;
    }

    public static void roll(float direction, float force) {
        //Send the message
        ServerMessage message = new ServerMessage(ServerMessageType.ROLL_SPHERO);
        message.addContent(direction);
        message.addContent(force);
        message.addContent(name);
        Server.send(message);
    }

    public static void shoot(float direction) {
        //Reset the ticks
        ticksSinceLastShot = 0;

        //Tell the server to shoot
        ServerMessage message = new ServerMessage(ServerMessageType.SPHERO_SHOOT);
        message.addContent(weapons[activeWeapon]);
        message.addContent(direction);
        message.addContent(name);
        Server.send(message);
    }

    public static void usePowerUp(int index) {
        ServerMessage message = new ServerMessage(ServerMessageType.SPHERO_POWER_UP);
        message.addContent(powerUps[index]);
        message.addContent(name);
        Server.sendAsync(message);
    }

    public static void pauseGame() {
        ServerMessage message = new ServerMessage(ServerMessageType.PAUSE_GAME);
        message.addContent(name);
        Server.sendAsync(message);
    }

    public static boolean getHasRecentDamage() {
        return recentDamage;
    }

    public static void setHasRecentDamage(boolean value) {
        recentDamage = value;
    }

    // Process input and send actions to the server. Should only be used to handle
    // events that are continuous e.g. rolling and shooting. One off actions such as
    // power up usage can just be called normally without the toggles.
    public static void startUpdateThread(final Callable<Void> func) {
        updateThreadIsRunning = true;
        updateThread = new Thread(new Runnable() {
            @Override
            public void run() {
                while (updateThreadIsRunning) {
                    try {
                        func.call();
                        Thread.sleep(UPDATE_MS_PER_TICK);   // Switch to nested update loop?
                    } catch (InterruptedException ex) {
                        Log.d(TAG, "Update thread interrupted.", ex);
                    } catch (Exception ex) {
                        Log.d(TAG, "Update input function exception.", ex);
                    }
                }
            }
        });

        updateThread.start();
    }

    public static void stopUpdateThread() {
        updateThreadIsRunning = false;
        try {
            updateThread.join();
        } catch (InterruptedException ex) {
            Log.d(TAG, "Update thread interrupted when joining.", ex);
        }
    }

    public static int getActiveWeapon() {
        return activeWeapon;
    }

    public static void setActiveWeapon(int index) {
        activeWeapon = index;
    }

    public static boolean weaponReady() { return ticksSinceLastShot >= ticksBetweenShots; }

    public static float getShield() {
        return shield;
    }

    public static void tick() { ticksSinceLastShot++; }

    public static float getHealth() {
        return health;
    }

    public static float getMaxHealth() {
        return maxHealth;
    }

    public static float getBatteryVoltage() {
        return voltage;
    }

    public static String getName() {
        return name;
    }

    public static void setName(String value) {
        name = value;
    }
}