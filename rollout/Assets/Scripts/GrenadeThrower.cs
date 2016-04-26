using UnityEngine;
using System.Collections;

public class GrenadeThrower : MonoBehaviour
{
    private int id = 102;
    public Grenade grenade;
    private Vector3 velocity;
    private Vector3 projectilePosition;

    public float speed;
    public float explosionRadius, explosionPower, maxDamage, minDamage;
    public float fireRate;
    public int ammunition = 20;
    public int maxAmmo = 10;

    public void Fire()
    {
        velocity = GetComponent<Rigidbody>().velocity;

        //If the ball is not moving then just aim NE
        if (velocity.magnitude == 0)
        {
            velocity = new Vector3(1f, 0f, 0f);
        }

        //Spawn the projectile outside of the player in the direction you are aiming
        projectilePosition = transform.position + velocity.normalized;
        var spawnedProjectile = (Grenade)Instantiate(grenade, projectilePosition, transform.rotation);
        spawnedProjectile.Initialise(velocity, speed, explosionRadius, explosionPower, maxDamage, minDamage);
    }

    public void Fire(Vector3 velocity)
    {
        //Spawn the projectile outside of the player in the direction you are aiming
        projectilePosition = transform.position + velocity.normalized;
        var spawnedProjectile = (Grenade)Instantiate(grenade, projectilePosition, transform.rotation);
        spawnedProjectile.Initialise(velocity, speed, explosionRadius, explosionPower, maxDamage, minDamage);

    }
    
}
