using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	private Vector3 velocity;
	public float speed;

	public int projectileDamage;
	private UniversalHealth health; // Access to UniversalHealth class

	//private ParticleSystem particles;


    /*void start()
    {
       Rigidbody rb = GetComponent<Rigidbody>();
    }*/

    public void Initialise(Vector3 givenVelocity)
	{
		velocity = givenVelocity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;

        Debug.Log(speed);
	}

    public void FixedUpdate()
    {
       // Rigidbody rb = GetComponent<Rigidbody>();
       // rb.velocity = velocity * speed;
    }

    void OnCollisionEnter(Collision col)
	{
		GameObject collidedObject = col.gameObject;
		health = collidedObject.GetComponent<UniversalHealth>();
		health.damagePlayer (projectileDamage);
		
		Destroy (gameObject);
	}
}