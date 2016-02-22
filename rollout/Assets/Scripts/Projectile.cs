using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Vector3 velocity;
	private GameObject playerShooting;
    public float speed;
    public int projectileDamage;
    private UniversalHealth health;
	private GameObject music;


    //private ParticleSystem particles;

	public void Initialise(Vector3 givenVelocity, GameObject player)
    {
        //Immediately make the projectile move in the desired direction
		playerShooting = player;
        velocity = givenVelocity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;



        //Destroys the projectile afer 2 seconds
        Destroy(gameObject, 2.0f);
		GetComponent<ParticleSystem> ().Play ();
    }

    void OnCollisionEnter(Collision col)
    {
		//print("collision player : " + col.gameObject.name + " player who spawned is : " + gameObject.name );
		if (col.gameObject.GetComponent<UniversalHealth> () && col.gameObject != playerShooting) {
			//Damage whatever collided with the projectile
			GameObject collidedObject = col.gameObject;

			health = collidedObject.GetComponent<UniversalHealth> ();
			health.damagePlayer (projectileDamage);

			music = GameObject.Find("Music");
			SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
			manager.CollideProjectile (collidedObject);

			Destroy (gameObject, 0.5f);
		} else {
			music = GameObject.Find("Music");
			SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
			manager.CollideObstacle (col.gameObject);
		}
    }

    void Update()
    {
    }
}