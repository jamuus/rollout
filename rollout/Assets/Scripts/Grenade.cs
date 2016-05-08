using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
    private float explosionRadius, explosionPower, maxDamage, minDamage;
    public Explosion explosion;

    public void Initialise(Vector3 velocity, float speed, float givenRadius, float givenPower, float givenMaxDamage, float givenMinDamage)
    {
        explosionRadius = givenRadius;
        explosionPower = givenPower;
        maxDamage = givenMaxDamage;
        minDamage = givenMinDamage;
        //Apply force to move the grenade
        Rigidbody rb = GetComponent<Rigidbody>();
        //rb.AddForce(velocity.normalized * speed); 
        rb.velocity = velocity.normalized * speed;


        //Schedule to destroy the missile after 3 seconds
        Destroy(gameObject, 3f);
    }

    void OnDestroy()
    {
        var spawnedExplosion = (Explosion)Instantiate(explosion, transform.position, transform.rotation);
        spawnedExplosion.Initialise(explosionRadius, explosionPower, maxDamage, minDamage);
        print("Explosion Successful");


	}
}
