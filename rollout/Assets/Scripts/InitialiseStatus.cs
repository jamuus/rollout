using UnityEngine;
using System.Collections;
using System;

public struct Status
{
    public string name;
    public string description;
    public float magnitude;
    public int time; //ms
}


public class InitialiseStatus: MonoBehaviour
{
    public Status[] statuses = new Status[5];

    void Start()
    {
        Status status;

        status.name = "Regeneration";
        status.description = "Continuously regenerates health of a player every second";
        status.magnitude = 1.0f;
        status.time = 15000;
        statuses[0] = status;

        status.name = "DoT";
        status.description = "Continuously deal damage to player";
        status.magnitude = 1.0f;
        status.time = 500;
        statuses[1] = status;

        status.name = "Slow down";
        status.description = "Player movement speed decreased";
        status.magnitude = 2.0f;
        status.time = 15000;
        statuses[2] = status;

        status.name = "Speed up";
        status.description = "Player movement speed increased";
        status.magnitude = 2.0f;
        status.time = 15000;
        statuses[3] = status;

        status.name = "Stun";
        status.description = "Player is unable to move";
        status.magnitude = 3.0f;
        status.time = (int)status.magnitude * 1000;
        statuses[4] = status;
    }
}

