using UnityEngine;
using System;
using System.Collections;

using Rewired;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    public float baseSpeed;
    public float maxSpeed;
    public float speed;
    public Vector3 velocity;
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

    public Sphero sphero;


    void Start()
    {
        speed = baseSpeed;
        velocity = GetComponent<Rigidbody> ().velocity;
        container = GameObject.Find("Container");
        statusList = container.GetComponent<InitialiseStatus>().statuses;
        allPowerUps = container.GetComponent<InitialisePowerUp>().powerUps;
        levelRadius = container.GetComponent<GenerateLevel>().levelRadius;
        boundaryHardness = container.GetComponent<GenerateLevel>().boundaryHardness * 2 + 1;
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

        // move ingame sphero
        #if !SOFTWARE_MODE
        if (sphero != null) {
            float moveHorizontal = sphero.Position.x;
            float moveVertical = -sphero.Position.y;
            // print(moveHorizontal);

            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 position = new Vector3(moveHorizontal, 0.5f, moveVertical);
            rb.position = position;

            // float X = player.GetAxis("Horizontalx");
            // float Y = player.GetAxis("Verticalx");
        }
        #endif
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
            rb.AddForce(speed * movement);
        }

        //Cap the max speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            print("Capping speed: " + rb.velocity.magnitude + " max speed = " + maxSpeed);
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }


    public void UsePowerUp(int powerUpID)
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

    public void AddPowerUp(PowerUp powerUp)
    {
        powerUps.Add (powerUp);
        sphero.PowerUps.Add(new SpheroPowerUp((SpheroPowerUpType)powerUp.id));//allPowerUps.IndexOf(powerUp)));

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
