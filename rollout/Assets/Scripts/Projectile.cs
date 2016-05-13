using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Vector3 velocity;
    private GameObject playerShooting;
    public float speed;
    public int damage;
    public Color colour = new Vector4(1, 0, 0, 1);
    private UniversalHealth health;
    private GameObject music;
	private AudioSource mains;
	public AudioClip collision;
	public AudioClip shield;

    //private ParticleSystem particles;
	public void SetSteroPan(AudioSource source, float x){
		float gridX = x;
		float posX = (gridX)/18.0f;
		source.panStereo = posX;
		float vol = (float) System.Math.Abs (posX) + 0.25f;
		if (vol > 1.0f) vol = 1.0f;
		else if (vol < 0.25f) vol = 0.25f;
		source.volume = vol;
	}

    public void Initialise(Vector3 givenVelocity)
    {
        mains = gameObject.GetComponent<AudioSource>();

        //set the colour
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", colour);
        
        //Immediately make the projectile move in the desired direction
        velocity = givenVelocity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;

        //Destroys the projectile afer 2 seconds
        Destroy(gameObject, 2.0f);
    }

    //In case you want to set your own speed, damage and colour
    public void Initialise(Vector3 givenVelocity, float givenSpeed, int givenDamage, Color givenColour)
    {
        //set the colour
        colour = givenColour;
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", givenColour);

        velocity = givenVelocity;
        speed = givenSpeed;
        damage = givenDamage;

        Initialise(givenVelocity);
    }

    //forces the projectile to ignore a specific collider
    public void ignoreCollider(Collider collider)
    {
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collider);
    }
    
    void OnCollisionEnter(Collision col)
    {
        //Destroy the game object
		float x = gameObject.transform.position.x;
		SetSteroPan (mains, x);
		if (col.gameObject.tag != "Shield") {
			Destroy (gameObject, 0.05f);
			mains.PlayOneShot (shield);
		} else {
			Destroy (gameObject, 2.0f);
			mains.PlayOneShot (collision);
		}

        //print("collision player : " + col.gameObject.name + " player who spawned is : " + gameObject.name );
        if (col.gameObject.GetComponent<UniversalHealth> () && col.gameObject != playerShooting)
        {
            //Damage whatever collided with the projectile
            GameObject collidedObject = col.gameObject;

            health = collidedObject.GetComponent<UniversalHealth> ();
            health.damagePlayer (damage);

        } else {
        }
    }
}
