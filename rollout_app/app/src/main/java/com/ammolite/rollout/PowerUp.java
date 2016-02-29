package com.ammolite.rollout;

import android.graphics.Color;

import org.json.JSONException;
import org.json.JSONObject;

public class PowerUp {
    public static final String KEY_ID           = "id";
    public static final String KEY_NAME         = "name";
    public static final String KEY_DESCRIPTION  = "description";
    public static final String KEY_COLOUR       = "colour";

    private int         id;
    private String      name;
    private String      description;
    private int         colour;

    public PowerUp(int id, String name, String description, int colour) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.colour = colour;
    }

    public PowerUp(JSONObject json) throws JSONException {
        id = json.getInt(KEY_ID);
        name = json.getString(KEY_NAME);
        description = json.getString(KEY_DESCRIPTION);
        colour = Color.parseColor(json.getString(KEY_COLOUR));
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
