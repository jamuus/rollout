package com.ammolite.rollout;

import android.graphics.Color;

public class GameEvent {
    private String  name;
    private String  description;
    private int     colour;
    private int     id;

    public GameEvent(int id, String name, String description, String colourString) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.colour = Color.parseColor(colourString);
    }

    public int getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public String getDescription() {
        return description;
    }

    public int getColour() {
        return colour;
    }
}
