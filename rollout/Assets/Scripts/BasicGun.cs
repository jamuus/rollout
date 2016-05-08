using UnityEngine;
using System.Collections;

public class BasicGun : MonoBehaviour
{
    public Projectile projectile;
    private Vector3 velocity;
    private Vector3 projectilePosition;
    public float fireRate = 0.5f;
    public float bulletSpeed = 40f;
    public int damage = 7;
    public Color colour = new Vector4(1, 0, 0, 1);
    public int ammunition = -1;
    public int maxAmmo = 1;

	private GameObject shield;

	void Start()
	{
		shield = gameObject.transform.Find("shield").gameObject;
	}

    public int ID()
    {
        return 100;
    }

    public void Fire ()
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
        spawnedProjectile.ignoreCollider(gameObject.GetComponent<Collider>());
        spawnedProjectile.ignoreCollider(shield.GetComponent<Collider>());
        spawnedProjectile.Initialise(velocity, bulletSpeed, damage, colour);
    }

    public void Fire(Vector3 velocity)
    {
        //Spawn the projectile outside of the player in the direction you are aiming
        projectilePosition = transform.position + velocity.normalized;
        var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
        spawnedProjectile.ignoreCollider(gameObject.GetComponent<Collider>());
        spawnedProjectile.ignoreCollider(gameObject.transform.Find("shield").GetComponent<Collider>());
        spawnedProjectile.Initialise(velocity, bulletSpeed, damage, colour);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
