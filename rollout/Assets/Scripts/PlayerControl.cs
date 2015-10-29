using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float speed;
<<<<<<< HEAD
    public Vector3 velocity;
    public string horizontalAxis;
    public string verticalAxis;
    void Start()
    {
        velocity = GetComponent<Rigidbody> ().velocity;
    }
=======
	public Vector3 velocity;
	public string horizontalAxis;
	public string verticalAxis;

	//public Projectile projectile;
   // private Vector3 projectilePosition;

	void Start()
	{
		velocity = GetComponent<Rigidbody> ().velocity;
	}
>>>>>>> refs/remotes/origin/Projectiles

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        velocity = rb.velocity;

        rb.AddForce(speed * movement);

    }

	//public void Update()
	//{
 //       if (Input.GetButtonDown ("Fire1"))
 //       {
 //           velocity = GetComponent<Rigidbody>().velocity;

 //           if (velocity.magnitude == 0)
 //           {
 //               velocity = new Vector3(0.5f, 0f, 0.5f);
 //           }

 //           projectilePosition = transform.position + velocity.normalized;
 //           var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);
 //           spawnedProjectile.Initialise(velocity);
 //       }
	//}
}
