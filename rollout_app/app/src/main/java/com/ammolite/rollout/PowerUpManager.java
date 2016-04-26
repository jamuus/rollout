package com.ammolite.rollout;

import android.content.Context;
import android.util.Log;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.HashMap;

public final class PowerUpManager {
    private static final String TAG = "PowerUpManager";

    private static HashMap<Integer, PowerUp> powerUps;

    static {
        powerUps = new HashMap<>();
    }

    private PowerUpManager() { }

    public static void parseJSON(String json) {
        try {
            JSONArray powerUpsArrayJson = new JSONArray(json);
            for (int i = 0; i < powerUpsArrayJson.length(); ++i) {
                JSONObject powerUpJson = powerUpsArrayJson.getJSONObject(i);
                PowerUp powerUp = new PowerUp(powerUpJson);
                powerUps.put(powerUp.getId(), powerUp);
            }
        } catch (JSONException ex) {
            Log.d(TAG, "Exception parsing JSON resource file.", ex);
        }
    }

    public static void loadAssetsFile(Context ctx) {
        try {
            InputStream stream = ctx.getAssets().open("powerups.json");
            BufferedReader reader = new BufferedReader(new InputStreamReader(stream, "UTF-8"));
            StringBuilder builder = new StringBuilder();
            String str;

            while ((str = reader.readLine()) != null)
                builder.append(str);

            reader.close();

            parseJSON(builder.toString());
        } catch (IOException ex) {
            Log.d(TAG, "Failed to open power ups assets file.", ex);
        }
    }

    public static PowerUp get(int id) {
        return powerUps.get(id);
    }
}
