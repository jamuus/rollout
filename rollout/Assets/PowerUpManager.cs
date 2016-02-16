using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SimpleJSON;

using UnityEngine;

public static class PowerUpManager
{
    public static Dictionary<int, PowerUp> PowerUps { get; private set; }

    public static void Initialise()
    {
        PowerUps = new Dictionary<int, PowerUp>();
        string json = File.ReadAllText("Assets/powerups.json");
        ParseJSON(json);
    }

    public static void ParseJSON(string json)
    {
        var powerUpsArrayJson = JSON.Parse(json);

        for (int i = 0; i < powerUpsArrayJson.Count; ++i)
            ParsePowerUpJSON(powerUpsArrayJson[i]);
    }

    private static void ParsePowerUpJSON(JSONNode json)
    {
        PowerUp p = new PowerUp();

        p.name = json["name"];
        p.description = json["description"];
        p.id = json["id"].AsInt;

        PowerUps[p.id] = p;
    }
}