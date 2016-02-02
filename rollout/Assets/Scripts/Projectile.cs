using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Vector3 velocity;
    public float speed;

    public int damage;
    private UniversalHealth health;

    //private ParticleSystem particles;

    public void Initialise(Vector3 givenVelocity)
    {
        //Immediately make the projectile move in the desired direction
        velocity = givenVelocity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;

        //Destroys the projectile afer 2 seconds
        Destroy(gameObject, 2.0f);
    }

    //In case you want to set your own speend and damage
    public void Initialise(Vector3 givenVelocity, float givenSpeed, int givenDamage)
    {
        velocity = givenVelocity;
        speed = givenSpeed;
        damage = givenDamage;

        //Immediately make the projectile move in the desired direction
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;

        //Destroys the projectile afer 2 seconds
        Destroy(gameObject, 2.0f);
    }


    void OnCollisionEnter(Collision col)
    {
		print("collision player : " + col.gameObject.name + " player who spawned is : " + gameObject.name );
		if (col.gameObject.GetComponent<UniversalHealth>()) {
            //Damage whatever collided with the projectile
            GameObject collidedObject = col.gameObject;
            health = collidedObject.GetComponent<UniversalHealth>();
            health.damagePlayer (damage);
        }
        Destroy(gameObject);
    }

    void Update()
    {
    }
}