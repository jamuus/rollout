package com.ammolite.rollout;

public final class ServerMessageType {
    private ServerMessageType() { }

    public static final int TEST            = 0x00;
    public static final int REMOVE_SPHERO   = 0x01;
    public static final int SET_ENDIANNESS  = 0x02;
    public static final int UPDATE_STATE    = 0x04;
    public static final int ROLL_SPHERO     = 0x08;
    public static final int SERVER_DISCOVER = 0x10;
    public static final int SPHERO_SHOOT    = 0x20;
    public static final int SPHERO_POWER_UP = 0x40;
    public static final int PAUSE_GAME      = 0x80;
    public static final int NODE_INIT       = 0x11;
    public static final int APP_INIT        = 0x21;
    public static final int SET_EVENTS      = 0x22;
    public static final int VOTE_EVENT      = 0x23;
}