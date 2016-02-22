using UnityEngine;
using System;
using System.Collections;

using Rewired;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
	public float baseSpeed;
    public float speed;
    public Vector3 velocity;
    public List<PowerUp> powerUps = new List<PowerUp>();
	public List<int> statuses = new List<int>(); //storing status time remaining
	private List<Status> statusList = new List<Status>();
    private GameObject container;
    public string horizontalAxis;
    public string verticalAxis;
    private Player player;
    public string powerUpButton;

    void Start()
    {
		speed = baseSpeed;
        velocity = GetComponent<Rigidbody> ().velocity;
        container = GameObject.Find("Container");
        statusList = container.GetComponent<InitialiseStatus>().statuses;
    }

    void Awake()
    {
        //Server.OpenConnection("127.0.0.1", 7777);
        //Server.SendEndianness();

        //Server.StartListening(7777);


        // get controller input
        // player = ReInput.players.GetPlayer(0);
    }

    void FixedUpdate()
    {
        Move();
        if (Input.GetButtonDown("Fire1")) UsePowerUp(0);
        triggerStatusEffects();


        Sphero sphero;
        if (SpheroManager.Instances.TryGetValue("tty.Sphero-YBR-AMP-SPP", out sphero)) {
            // move ingame sphero
            float moveHorizontal = sphero.Position.x;
            float moveVertical = sphero.Position.y;

            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 position = new Vector3(moveHorizontal, 0.5f, moveVertical);
            rb.position = position;

            // Debug.Log(string.Format("{0}, {1}", moveHorizontal, moveVertical));

            // // get controller input and omve sphero
            // float X = player.GetAxis("Horizontalx");
            // float Y = player.GetAxis("Verticalx");

            // float force = Mathf.Sqrt(Mathf.Pow(X, 2)
            //                          + Mathf.Pow(Y, 2));
            // float direction = Vector2.Angle(new Vector2(0, 1), new Vector2(X, Y));
            // if (X < 0) direction = -direction + 360.0f;
            // // direction = Mathf.Rad2Deg * direction;
            // get controller input and omve sphero
            float X = player.GetAxis("Horizontalx");
            float Y = player.GetAxis("Verticalx");

            // sphero.Roll(direction, force);
        }

    }

    // Debug.Log(string.Format("{0}, {1}", controllerHorizontal, controllerVertical));
    // Debug.Log(string.Format("{0}", player));


    void OnApplicationQuit()
    {
        //Server.CloseConnection();
        //Server.StopListening();
    }


    void Move()
    {

        Rigidbody rb = GetComponent<Rigidbody>();
        float radius = 11f;
		bool outOfBounds = radius < rb.position.magnitude; // check if left arena

        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        velocity = rb.velocity;
        if (outOfBounds) {
			// accelerate in opposite direction
            rb.AddForce(baseSpeed * (radius - rb.position.magnitude) * rb.position.normalized);
        } else {
            rb.AddForce(speed * movement);
        }
    }


    void UsePowerUp(int powerUpID)
    {
        PowerUp usedPowerUp;
        try {
            usedPowerUp = powerUps[0];
            powerUpEffect(usedPowerUp);
            powerUps.RemoveAt(0);
            print("PowerUp " + usedPowerUp.name + " used by player");
        } catch (Exception e) {
            print("No powerups left");
        }
    }

    public void AddPowerUp(PowerUp powerUp)
    {
        powerUps.Add (powerUp);
		print("PowerUp " + powerUp.name + " added to " + gameObject.name);
    }

    public void powerUpEffect(PowerUp powerUp)
    {
		GameObject otherPlayer = GameObject.Find(gameObject.name == "player1" ? "player2" : "player1");

		if (powerUp.name == "Regeneration") {
			statuses [0] = statusList[0].time;
		}
		if (powerUp.name == "Damage Enemy") {
			otherPlayer.GetComponent<PlayerControl>().statuses [1] = statusList[1].time;
		}
		if (powerUp.name == "Slow Down Enemy") {
			otherPlayer.GetComponent<PlayerControl>().statuses [2] = statusList[2].time;
			statuses [3] = 0;
		}
        if (powerUp.name == "Boost") {
            statuses [3] = statusList[3].time;
			statuses [2] = 0; // When boost used it removes negative speed effects
			statuses [4] = 0; // Removing stunn status
        }
		if (powerUp.name == "Stun Enemy") {
			otherPlayer.GetComponent<PlayerControl>().statuses [4] = statusList[4].time;
		}
    }

    private void triggerStatusEffects()
    {
		//regeneration
        if (statuses[0] > 0) {
            this.GetComponent<UniversalHealth>().healPlayer((int)statusList[0].magnitude);
			decrementStatusDuration(0);
			if (statuses[0] <= 0) print ("End of regeneration");
        }

		//damage over time
		if (statuses[1] > 0) {
			this.GetComponent<UniversalHealth>().damagePlayer((int)statusList[0].magnitude);
			decrementStatusDuration(1);
			if (statuses[1] <= 0) print ("End of damage");
		}

		// reduce speed
		if (statuses[2] > 0) {
			if (statuses[2] == statusList[2].time) speed = baseSpeed / statusList[2].magnitude;
			decrementStatusDuration(2);
			if (statuses[2] <= 0) {
				speed = baseSpeed;
				print ("End of slow down");
			}
		}

		// increase speed
        if (statuses[3] > 0) {
			if (statuses[3] == statusList[3].time) speed = baseSpeed * statusList[3].magnitude;
			decrementStatusDuration(3);
            if (statuses[3] <= 0) {
                speed = baseSpeed;
                print ("End of speed boost");
            }
        }

		// stun
		if (statuses[4] > 0) {
			//print ("stun duration remaining: " + statuses [4]);
			speed = 0;
			decrementStatusDuration(4);
			if (statuses [4] <= 0) {
				speed = baseSpeed;
				print ("End of stun");
			}
		}
    }
	private void decrementStatusDuration(int statusID)
	{
		statuses[statusID] -= 25;
	}

}