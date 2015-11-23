using UnityEngine;
using System.Collections;

public class ProjectileControl : MonoBehaviour
{
    private Vector3 velocity;
    public Projectile projectile;
    private Vector3 projectilePosition;

    void Start()
    {
        velocity = GetComponent<Rigidbody>().velocity;
    }


    public void Update()
    {
        if ((Input.GetButtonDown("Fire1") && gameObject.name == ("player1")) || (Input.GetButtonDown("Fire2") && gameObject.name == ("player2")))
        {
            velocity = GetComponent<Rigidbody>().velocity;
      
            //if the ball is not moving then just aim NE
            if (velocity.magnitude == 0)
            {
                velocity = new Vector3(0.5f, 0f, 0.5f);
            }

            //spawn the projectile outside of the player in the direction you are aiming
            projectilePosition = transform.position + velocity.normalized;
            var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
            spawnedProjectile.Initialise(velocity);
        }
    }
        
}
