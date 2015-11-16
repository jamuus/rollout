using UnityEngine;
using System.Collections;

public class HomingMissile : MonoBehaviour
{
    private Vector3 velocity;
    public float speed;
    public float turnSpeed;

    public int projectileDamage;
    private Rigidbody homingMissile;
    private UniversalHealth health;
    private GameObject otherPlayer;

    public void Initialise(Vector3 givenVelocity, GameObject givenOtherPlayer)
    {
        otherPlayer = givenOtherPlayer;

        //Immediately make the projectile move in the desired direction
        //velocity = givenVelocity;
        homingMissile = GetComponent<Rigidbody>();
        //homingMissile.velocity = velocity.normalized * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        //Damage whatever collided with the projectile
        GameObject collidedObject = col.gameObject;
        health = collidedObject.GetComponent<UniversalHealth>();
        health.damagePlayer(projectileDamage);

        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        //Rotates the missile towards the other player
        var targetRotation = Quaternion.LookRotation(otherPlayer.transform.position - transform.position);
        homingMissile.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed));
        homingMissile.velocity = transform.forward * speed;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, otherPlayer.transform.rotation, 2.0f);

    }
}
