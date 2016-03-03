using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Vector3 velocity;
    private GameObject playerShooting;
    public float speed;
    public int damage;

    private UniversalHealth health;
    private GameObject music;


    //private ParticleSystem particles;

    public void Initialise(Vector3 givenVelocity)
    {
        //Immediately make the projectile move in the desired direction
        velocity = givenVelocity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;

        //Destroys the projectile afer 2 seconds
        Destroy(gameObject, 2.0f);
        //GetComponent<ParticleSystem> ().Play ();
    }

    //In case you want to set your own speed and damage
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
        //print("collision player : " + col.gameObject.name + " player who spawned is : " + gameObject.name );
        if (col.gameObject.GetComponent<UniversalHealth> () && col.gameObject != playerShooting) {
            //Damage whatever collided with the projectile
            GameObject collidedObject = col.gameObject;

            health = collidedObject.GetComponent<UniversalHealth> ();
            health.damagePlayer (damage);

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
