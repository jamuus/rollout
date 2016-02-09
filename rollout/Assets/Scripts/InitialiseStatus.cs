using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public struct Status
{
    public string name;
    public string description;
    public float magnitude;
    public int time; //ms
}


public class InitialiseStatus: MonoBehaviour
{
	public List<Status> statuses = new List<Status>();

    void Start()
    {
        Status status;

        status.name = "Regeneration";
        status.description = "Continuously regenerates health of a player every second";
        status.magnitude = 1.0f;
        status.time = 15000;
		statuses.Add(status);

        status.name = "DoT";
        status.description = "Continuously deal damage to player";
        status.magnitude = 1.0f;
        status.time = 500;
		statuses.Add(status);

        status.name = "Slow down";
        status.description = "Player movement speed decreased";
        status.magnitude = 2.0f;
        status.time = 15000;
		statuses.Add(status);

        status.name = "Speed up";
        status.description = "Player movement speed increased";
        status.magnitude = 2.0f;
        status.time = 15000;
		statuses.Add(status);

        status.name = "Stun";
        status.description = "Player is unable to move";
        status.magnitude = 3.0f;
        status.time = (int)status.magnitude * 1000;
		statuses.Add(status);
    }

	public List<Status> getStatuses()
	{
		return statuses;
	}
}

