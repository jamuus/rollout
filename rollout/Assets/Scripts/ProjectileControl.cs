using UnityEngine;
using System.Collections;

public class ProjectileControl : MonoBehaviour
{
    private Vector3 velocity;
    public Projectile projectile;
    public HomingMissile homingMissile;
    private Vector3 projectilePosition;
    private GameObject otherPlayer;

    void Start()
    {
        velocity = GetComponent<Rigidbody>().velocity;

        if (gameObject.name == "player1")
        {
            otherPlayer = GameObject.Find("player2");
        }
        else if (gameObject.name == "player2")
        {
            otherPlayer = GameObject.Find("player1");
        }
    }
    
    public void Update()
    {
        //If the fire button for this sphero gets pressed
        if ((Input.GetButtonDown("Fire1") && gameObject.name == ("player1")) || (Input.GetButtonDown("Fire2") && gameObject.name == ("player2")))
        {
            velocity = GetComponent<Rigidbody>().velocity;
      
            //If the ball is not moving then just aim NE
            if (velocity.magnitude == 0)
            {
                velocity = new Vector3(1f, 0f, 0f);
            }

            //Spawn the projectile outside of the player in the direction you are aiming
            projectilePosition = transform.position + velocity.normalized;
            var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
            spawnedProjectile.Initialise(velocity);
        }


        if (Input.GetButtonDown("Fire3") && gameObject.name == ("player1"))
        {
            velocity = otherPlayer.transform.position - transform.position;
            projectilePosition = transform.position + velocity.normalized;
            var spawnedMissile = (HomingMissile)Instantiate(homingMissile, projectilePosition, transform.rotation);
            spawnedMissile.Initialise(velocity, otherPlayer);
        }
    }
        
}        

