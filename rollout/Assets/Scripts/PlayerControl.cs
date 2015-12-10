using UnityEngine;
using System;
using System.Collections;

using Rewired;
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

    private Player player;
    public string powerUpButton;

    void Start()
    {
        velocity = GetComponent<Rigidbody> ().velocity;
        container = GameObject.Find("Container");
        statusList = container.GetComponent<InitialiseStatus>().statuses;
    }


    void Awake()
    {
        Server.OpenConnection("127.0.0.1", 7777);
        Server.SendEndianness();

        player = ReInput.players.GetPlayer(0);
    }

    void FixedUpdate()
    {
        Move ();
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

            // get controller input and omve sphero
            float X = player.GetAxis("Horizontalx");
            float Y = player.GetAxis("Verticalx");

            float force = Mathf.Sqrt(Mathf.Pow(X, 2)
                                     + Mathf.Pow(Y, 2));
            float direction = Vector2.Angle(new Vector2(0, 1), new Vector2(X, Y));
            if (X < 0) direction = -direction + 360.0f;
            // direction = Mathf.Rad2Deg * direction;

            sphero.Roll(direction, force);
        }
    }

    // Debug.Log(string.Format("{0}, {1}", controllerHorizontal, controllerVertical));
    // Debug.Log(string.Format("{0}", player));


    void OnApplicationQuit()
    {
        Server.CloseConnection();
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
        if (outOfBounds) {
            print ("" + rb.velocity.magnitude);
            rb.AddForce(speed * (radius - rb.position.magnitude) * rb.position.normalized);
        } else {
            rb.AddForce(speed * movement);
        }
    }

    void UsePowerUp(int powerUpID)
    {
        PowerUp usedPowerUp;
        try {
            usedPowerUp = powerUps[powerUps.Count - 1];
            powerUpEffect(usedPowerUp);
            powerUps.RemoveAt(powerUps.Count - 1);
            print("PowerUp " + usedPowerUp.name + " used by player");
        } catch (Exception e) {
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

        if (powerUp.name == "Boost") {
            statuses[3] = statusList[3].time;
        }
        if (powerUp.name == "Regeneration") {
            statuses[0] = statusList[0].time;
        }
    }

    private void triggerStatusEffects()
    {
        if (statuses[0] > 0) {
            this.GetComponent<UniversalHealth>().healPlayer((int)statusList[0].magnitude);
            statuses[0] -= 20;
            print ("End of speed regeneration");
        }
        if (statuses[3] > 0) {
            if (statuses[3] == statusList[3].time) speed *= statusList[3].magnitude;
            statuses[3] -= 20;
            if (statuses[3] <= 0) {
                speed /= 2;
                print ("End of speed boost");
            }
        }
    }

}