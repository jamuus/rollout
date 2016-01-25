using UnityEngine;
using System.Collections;

public class ProjectileControl : MonoBehaviour
{
    private Vector3 velocity;
    public Projectile projectile;

    public HomingMissile homingMissile;
    private Vector3 projectilePosition;
    private GameObject otherPlayer;
    private Quaternion projectileRotation;


    void Start()
    {
        velocity = GetComponent<Rigidbody>().velocity;

        if (gameObject.name == "player1") {
            otherPlayer = GameObject.Find("player2");
        } else if (gameObject.name == "player2") {
            otherPlayer = GameObject.Find("player1");
        }
    }

    public void Update()
    {
        //If the fire button for this sphero gets pressed
        if ((Input.GetButtonDown("Fire1") && gameObject.name == ("player1")) || (Input.GetButtonDown("Fire2") && gameObject.name == ("player2"))) {
            velocity = GetComponent<Rigidbody>().velocity;

            //If the ball is not moving then just aim NE
            if (velocity.magnitude == 0) {
                velocity = new Vector3(1f, 0f, 0f);
            }


            //Spawn the projectile outside of the player in the direction you are aiming
            projectilePosition = transform.position + velocity.normalized;
            var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);

            spawnedProjectile.Initialise(velocity);
        }


        if (Input.GetButtonDown("Fire3") && gameObject.name == ("player1")) {
            //if the other player is alive, set the direction of the missile towards it, otherwise set it in direction of movement
            if (otherPlayer != null) {
                //Set the spawn position and rotation to be towards the other player
                velocity = otherPlayer.transform.position - transform.position;
                projectileRotation = Quaternion.LookRotation(otherPlayer.transform.position - transform.position, Vector3.up);
            } else {
                //Set the spawn position and rotation to be in the direction of movement
                velocity = GetComponent<Rigidbody>().velocity;
                projectileRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            }

			projectilePosition = transform.position + (velocity.normalized);
            var spawnedMissile = (HomingMissile)Instantiate(homingMissile, projectilePosition, projectileRotation);
            spawnedMissile.Initialise(velocity, otherPlayer);
        }
    }


}

