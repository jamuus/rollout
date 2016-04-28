using UnityEngine;
using System;
using System.Collections;

using Rewired;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
	public int gameStateId;
    public float baseSpeed = 8;
    public float maxSpeed = 20;
    public float speed;
    public Vector3 velocity;
	private Vector3 startingPosition;

    public List<PowerUp> powerUps = new List<PowerUp>();
    public List<int> statuses = new List<int>(); //storing status time remaining
    private List<Status> statusList = new List<Status>();
    private GameObject container;
    public string horizontalAxis;
    public string verticalAxis;
    private GameObject music;

    private Player player;
    public string powerUpButton;
    private List<PowerUp> allPowerUps;
    private float levelRadius;
    private int boundaryHardness;

	public ParticleSystem[] particles;

    public Sphero sphero;


    void Start()
    {
		startingPosition = gameObject.transform.position;
        speed = baseSpeed;
        velocity = GetComponent<Rigidbody> ().velocity;
        container = GameObject.Find("Container");
        statusList = container.GetComponent<InitialiseStatus>().statuses;
        allPowerUps = container.GetComponent<InitialisePowerUp>().powerUps;
        levelRadius = container.GetComponent<GenerateLevel>().levelRadius;
        boundaryHardness = container.GetComponent<GenerateLevel>().boundaryHardness * 2 + 1;

		particles = gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem ps in particles) {
			//ps.Play ();
		}
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

		Rigidbody rb = GetComponent<Rigidbody>();
		Vector3 position;


		Move ();
		if (gameStateId == 0 || (gameStateId == 1 && gameObject.GetComponent<UniversalHealth> ().currentHealth > 0)) {
			if ((Input.GetButtonDown ("Fire1") && gameObject.name == "player1") ||
			    (Input.GetButtonDown ("Fire2") && gameObject.name == "player2")) {
				UsePowerUp ();
			}
			triggerStatusEffects ();
		}

        // move ingame sphero
        #if !SOFTWARE_MODE
        if (sphero != null) {
            float moveHorizontal = sphero.Position.x;
			float moveVertical = -sphero.Position.y;
            // print(moveHorizontal);

            position = new Vector3(moveHorizontal, 0.9000168f, moveVertical);
            //rb.position = position;
            rb.MovePosition(position);

            // float X = player.GetAxis("Horizontalx");
            // float Y = player.GetAxis("Verticalx");
        }
        #endif
		if (gameStateId == 2) {
			resetPlayerStatus ();
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
        float distanceFromEdge = levelRadius - (rb.position.magnitude * 1.1f); // check if left arena

        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);
#if SOFTWARE_MODE
        if (sphero != null)
        {
            moveHorizontal += sphero.Force.x;
            moveVertical   += sphero.Force.y;
        }
#endif

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        velocity = rb.velocity;
        if (distanceFromEdge < 0)
        {
            // accelerate in opposite direction
            rb.AddForce(speed * 2 * (float)Math.Pow(distanceFromEdge, boundaryHardness) * rb.position.normalized);
        }
        else
        {
			if (gameStateId == 0 || (gameStateId == 1 && gameObject.GetComponent<UniversalHealth> ().currentHealth > 0)) {
				rb.AddForce (speed * movement);
			}
        }

        //Cap the max speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            print("Capping speed: " + rb.velocity.magnitude + " max speed = " + maxSpeed);
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        // sphero.EnvironmentForce -= GetComponent<Rigidbody>().velocity;
		if (sphero != null)
		sphero.EnvironmentForce += collision.relativeVelocity;
    }

    public void OnCollisionStay(Collision collision)
    {
		if (sphero != null)
        sphero.EnvironmentForce += collision.relativeVelocity;
    }

    public void OnCollisionExit(Collision collision)
    {
        Debug.LogFormat("END COLLISION");
    }

    public void LateUpdate()
    {
        if (sphero != null && name == "player1")
        {
            Debug.LogFormat("Env: {0}", sphero.EnvironmentForce);
            Debug.DrawRay(transform.position, sphero.EnvironmentForce, Color.red);
        }

		if (sphero != null)
        sphero.EnvironmentForce *= 0.9f;
    }

    public void UsePowerUp()
    {
        PowerUp usedPowerUp;
        try {
            usedPowerUp = powerUps[0];
            powerUpEffect(usedPowerUp);
            powerUps.RemoveAt(0);
            print("PowerUp " + usedPowerUp.name + " used by player");

            //sphero.PowerUps.RemoveAt(0);
        } catch (Exception e) {
            print("No powerups left");
        }
    }

	//for the app
	public void UsePowerUp(int powerUpId)
	{
		PowerUp usedPowerUp;
		try {
			usedPowerUp = powerUps[powerUpId];
			powerUpEffect(usedPowerUp);
			powerUps.RemoveAt(powerUpId);
			print("PowerUp " + usedPowerUp.name + " used by player");

			//sphero.PowerUps.RemoveAt(0);
		} catch (Exception e) {
			print("No powerups left");
		}
	}

    public void AddPowerUp(PowerUp powerUp)
    {
        powerUps.Add (powerUp);
        //this line breaks the powerUp behavior in game - the field doesn't time out
		try {
			sphero.PowerUps.Add(new SpheroPowerUp((SpheroPowerUpType)powerUp.id));//allPowerUps.IndexOf(powerUp)));
		}
		catch {}
        music = GameObject.Find("Music");
        SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
        manager.PickPowerUp (gameObject);
        print("PowerUp " + powerUp.name + " added to " + gameObject.name);
    }

    public void powerUpEffect(PowerUp powerUp)
    {
        GameObject otherPlayer = GameObject.Find(gameObject.name == "player1" ? "player2" : "player1");

        if (powerUp.name == "Regeneration") {
            statuses [0] = statusList[0].time;
			particles [0].Play ();
        }
        if (powerUp.name == "Damage Enemy") {
            otherPlayer.GetComponent<PlayerControl>().statuses [1] = statusList[1].time;
			otherPlayer.GetComponent<PlayerControl>().particles [1].Play ();
        }
        if (powerUp.name == "Slow Down Enemy") {
            otherPlayer.GetComponent<PlayerControl>().statuses [2] = statusList[2].time;
            statuses [3] = 0;
			otherPlayer.GetComponent<PlayerControl>().particles [2].Play ();
        }
        if (powerUp.name == "Boost") {
            statuses [3] = statusList[3].time;
            statuses [2] = 0; // When boost used it removes negative speed effects
            statuses [4] = 0; // Removing stunn status
			particles [3].Play ();
        }
        if (powerUp.name == "Stun Enemy") {
            otherPlayer.GetComponent<PlayerControl>().statuses [4] = statusList[4].time;
			otherPlayer.GetComponent<PlayerControl>().particles [4].Play ();
        }
        if (powerUp.name == "Boost")
        {
            //Apply a force to the sphero
            velocity += powerUp.value*velocity.normalized;
        }
    }

    private void triggerStatusEffects()
    {
        //regeneration
        if (statuses[0] > 0) {
			if ((statuses [1] % 100) == 0) {
				this.GetComponent<UniversalHealth> ().healPlayer ((int)statusList [0].magnitude);
			}
			decrementStatusDuration(0);
			if (statuses [0] <= 0) {
				print ("End of regeneration");
				particles [0].Stop ();
			}
        }

        //damage over time
		if (statuses [1] > 0) {
			this.GetComponent<UniversalHealth> ().damagePlayer ((int)statusList [0].magnitude);
			decrementStatusDuration (1);
			if (statuses [1] <= 0) {
				print ("End of damage");
				particles [1].Stop ();
			}
		}

        // reduce speed
        if (statuses[2] > 0) {
            if (statuses[2] == statusList[2].time) speed = baseSpeed / statusList[2].magnitude;
            decrementStatusDuration(2);
            if (statuses[2] <= 0) {
				particles [2].Stop ();
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
				particles [3].Stop ();

            }
        }

        // stun
        if (statuses[4] > 0) {
            //print ("stun duration remaining: " + statuses [4]);
            speed = 0;
            decrementStatusDuration(4);
            if (statuses [4] <= 0) {
				particles [4].Stop ();
                speed = baseSpeed;
                print ("End of stun");
            }
        }
    }
    private void decrementStatusDuration(int statusID)
    {
        statuses[statusID] -= 25;
    }

	private void resetPlayerStatus()
	{
		foreach (ParticleSystem ps in particles) {
			ps.Stop ();
		}
		for (int i=0; i < statuses.Count; i++) {
			statuses [i] = 0;
		}

		powerUps.Clear ();
		speed = baseSpeed;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		gameObject.GetComponent<UniversalHealth> ().currentHealth = gameObject.GetComponent<UniversalHealth> ().getMaxHealth ();
		gameObject.transform.position = startingPosition;
	}
}
