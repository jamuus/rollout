package com.ammolite.rollout;

import java.util.HashMap;

public final class GameEventHelper {
    private GameEventHelper() { }

    private static HashMap<Integer, GameEvent> eventsMap;

    static {
        eventsMap = new HashMap<>();

        // Hardcoded events for now, should probably load from a config file
        // as with power ups.
        eventsMap.put(0, new GameEvent(0, "Hell", "Spawn a lot of lava.", "#ff0000"));
        eventsMap.put(1, new GameEvent(1, "Earthquake", "Shakes the spheros up.", "#995555"));
        eventsMap.put(2, new GameEvent(2, "Enemy", "Spawn a vicious enemy that shoots at players.", "#00ff66"));
    }

    public static GameEvent get(int id) {
        return eventsMap.get(id);
    }
}
