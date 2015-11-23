using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	private Vector3 velocity;
	public float speed;

	public int projectileDamage;
	private UniversalHealth health; 

	//private ParticleSystem particles;

    public void Initialise(Vector3 givenVelocity)
	{
        //Immediately make the projectile move in the desired direction
		velocity = givenVelocity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;
	}

    void OnCollisionEnter(Collision col)
	{
        //Damage whatever collided with the projectile
		GameObject collidedObject = col.gameObject;
		health = collidedObject.GetComponent<UniversalHealth>();
		health.damagePlayer (projectileDamage);
		
		Destroy (gameObject);
	}

    void Update()
    {
        //Destroys the projectile afer 1.3 seconds
        Destroy(gameObject, 1.3f);
    }
}