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
        homingMissile = GetComponent<Rigidbody>();

        //Schedule to destroy the missile after 8 seconds
        Destroy(gameObject, 8f);
    }

    void OnCollisionEnter(Collision col)
    {
        GameObject collidedObject = col.gameObject;

        if (collidedObject.name == "player1" || collidedObject.name == "player2")
        {
            //Damage whatever collided with the projectile
            health = collidedObject.GetComponent<UniversalHealth>();
            health.damagePlayer(projectileDamage);
        }
        Destroy(gameObject);
    }
	
	void Update ()
    {
	
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
}
