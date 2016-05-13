using UnityEngine;
using System.Collections;

public class Shotgun : MonoBehaviour
{
    public Projectile projectile;
    private Vector3 velocity;
    private Vector3 projectilePosition;
    public float fireRate = 1.0f;
    public float bulletSpeed = 40f;
    public int damage = 3;
    public int noOfPellets = 9;
    public float spreadAngle = 15;
    public int ammunition = 5;
    public int maxAmmo = 60;
    public Color colour = new Vector4(0f, 1f, 0f, 1f);

    public int ID()
    {
        return 103;
    }

    public void Fire()
    {
        velocity = GetComponent<Rigidbody>().velocity.normalized;

        //If the ball is not moving then just aim NE
        if (velocity.magnitude == 0)
        {
            velocity = new Vector3(1f, 0f, 0f);
        }

        for(int i = 0; i < noOfPellets; i++)
        {
            projectilePosition = transform.position + velocity.normalized;

            float angleOffset = Random.Range(-spreadAngle/2, spreadAngle/2);
            velocity = Quaternion.Euler(0, -angleOffset, 0) * velocity;

            var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
            spawnedProjectile.ignoreCollider(gameObject.GetComponent<Collider>());
            spawnedProjectile.GetComponent<SphereCollider>().isTrigger = false;
            spawnedProjectile.Initialise(velocity, bulletSpeed, damage, colour);
        }

    }

    public void Fire(Vector3 velocity)
    {
        //Spawn the projectile outside of the player in the direction you are aiming
        projectilePosition = transform.position + velocity.normalized;
        var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
        spawnedProjectile.ignoreCollider(gameObject.GetComponent<Collider>());
        spawnedProjectile.Initialise(velocity, bulletSpeed, damage, colour);

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
