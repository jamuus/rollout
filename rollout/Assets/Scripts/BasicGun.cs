using UnityEngine;
using System.Collections;

public class BasicGun : MonoBehaviour {
    public Projectile projectile;
    private Vector3 velocity;
    private Vector3 projectilePosition;
    public float fireRate;

    public void Fire ()
    {
        velocity = GetComponent<Rigidbody>().velocity;

        //If the ball is not moving then just aim NE
        if (velocity.magnitude == 0)
        {
            velocity = new Vector3(1f, 0f, 0f);
        }

        //PROJECTILE POSIION TO BE CHANGED ONCE WE GET AIM DIRECTION INFO
        //Spawn the projectile outside of the player in the direction you are aiming
        projectilePosition = transform.position + velocity.normalized;
        var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
        spawnedProjectile.ignoreCollider(gameObject.GetComponent<Collider>());
        spawnedProjectile.Initialise(velocity);
        print("gun shot");
    }

    public void Fire(Vector3 velocity)
    {
        //Spawn the projectile outside of the player in the direction you are aiming
        projectilePosition = transform.position + velocity.normalized;
        var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
        spawnedProjectile.ignoreCollider(gameObject.GetComponent<Collider>());
        spawnedProjectile.Initialise(velocity);

    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
