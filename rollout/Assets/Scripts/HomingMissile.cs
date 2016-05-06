using UnityEngine;
using System.Collections;

public class HomingMissile : MonoBehaviour
{
    private Vector3 velocity;
    public float speed;
    public float turnSpeed;
    public float explosionPower;

    public int damage;
    public Explosion explosion;
    private Rigidbody homingMissile;
    private GameObject otherPlayer;
	private AudioSource mains;
	public AudioClip collision;

    public void Initialise(Vector3 givenVelocity, GameObject givenOtherPlayer)
    {
        otherPlayer = givenOtherPlayer;
        homingMissile = gameObject.GetComponent<Rigidbody>();
        mains = gameObject.GetComponent<AudioSource>();

        //Schedule to destroy the missile after 8 seconds
        Destroy(gameObject, 8f);
    }

    //In case you want to set your own speed and damage
    public void Initialise(Vector3 givenVelocity, float givenSpeed, float givenTurnSpeed, int givenDamage, GameObject givenOtherPlayer)
    {
        otherPlayer = givenOtherPlayer;
        velocity = givenVelocity;
        speed = givenSpeed;
        turnSpeed = givenTurnSpeed;
        damage = givenDamage;

        //Immediately make the projectile move in the desired direction
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = velocity.normalized * speed;

        Initialise(givenVelocity, givenOtherPlayer);
    }

    //when the missile hits something, spawn an explosion
    void OnCollisionEnter(Collision col)
    {
        //var spawnedExplosion = (Explosion)Instantiate(explosion, transform.position, transform.rotation);
        //spawnedExplosion.Initialise(4, explosionPower, 30, 10);
        //print("Explosion Successful");
        if (col.gameObject.tag != "Shield") Destroy(gameObject);
        else
        {
            deflect();
            Destroy(gameObject, 2.0f);
        }
    }

    void deflect()
    {
        //Aim towards the player that fired it
        otherPlayer = GameObject.Find(otherPlayer.name.Contains("2") ? "player1" : "player2");

        //Reverse the direction
        var targetRotation = Quaternion.LookRotation(otherPlayer.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Euler(new Vector3(0, homingMissile.rotation.y + 180f, 0));
    }

    void FixedUpdate()
    {
        if (otherPlayer != null)
        {
            //Rotates the missile towards the other player
            var targetRotation = Quaternion.LookRotation(otherPlayer.transform.position - transform.position, Vector3.up);
            homingMissile.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed));
        }
        homingMissile.velocity = transform.forward * speed;
    }

    void OnDestroy()
    {
        var spawnedExplosion = (Explosion)Instantiate(explosion, transform.position, transform.rotation);
        spawnedExplosion.Initialise(4, explosionPower, 30, 10);
        mains.PlayOneShot(collision);
        print("Explosion Successful");
    }
}
