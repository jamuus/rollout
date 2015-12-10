using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public Vector3 velocity;
    private List<PowerUp> powerUps = new List<PowerUp>();
    private int[] statuses = new int[5]; //storing status time remaining
    private Status[] statusList = new Status[5];
    private GameObject container;
    public string horizontalAxis;
    public string verticalAxis;
    public string powerUpButton;

	void Start()
	{
		velocity = GetComponent<Rigidbody> ().velocity;
        container = GameObject.Find("Container");
        statusList = container.GetComponent<InitialiseStatus>().statuses;
	}

    void FixedUpdate()
    {
        Move ();
        if (Input.GetButtonDown("Fire1")) UsePowerUp(0);
        triggerStatusEffects();
    }

    void Move()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float radius = 11f;
        bool outOfBounds = radius < rb.position.magnitude;
        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);     
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        velocity = rb.velocity;
        if (outOfBounds)
        {
            print ("" + rb.velocity.magnitude);
            rb.AddForce(speed * (radius - rb.position.magnitude) * rb.position.normalized);
        }
        else
        {
            rb.AddForce(speed * movement);
        }
    }

    void UsePowerUp(int powerUpID)
    {
        PowerUp usedPowerUp;
        try
        {
            usedPowerUp = powerUps[powerUps.Count - 1];
            powerUpEffect(usedPowerUp);
            powerUps.RemoveAt(powerUps.Count - 1);
            print("PowerUp " + usedPowerUp.name + " used by player");
        }
        catch (Exception e)
        {
            print("No powerups left");
        }

    }
    public void AddPowerUp(PowerUp powerUp)
    {
        powerUps.Add (powerUp);
        print("PowerUp " + powerUp.name + " added to player");
    }
    private void powerUpEffect(PowerUp powerUp)
    {

        if (powerUp.name == "Boost")
        {
            statuses[3] = statusList[3].time;
        }
        if (powerUp.name == "Regeneration")
        {
            statuses[0] = statusList[0].time;
        }
    }

    private void triggerStatusEffects()
    {
        if (statuses[0] > 0)
        {
            this.GetComponent<UniversalHealth>().healPlayer((int)statusList[0].magnitude);
            statuses[0] -= 20;
            print ("End of speed regeneration");
        }
        if (statuses[3] > 0)
        {
            if (statuses[3] == statusList[3].time) speed *= statusList[3].magnitude;
            statuses[3] -=20;
            if (statuses[3] <= 0)
            {
                speed /= 2;
                print ("End of speed boost");
            }
        }
    }
}
