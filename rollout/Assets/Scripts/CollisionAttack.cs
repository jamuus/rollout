using UnityEngine;
using System.Collections;
using System;

public class CollisionAttack : MonoBehaviour
{
    public int collisionAttackValue = 10; //Base attack value
    private UniversalHealth health; // Access to UniversalHealth class
    private ParticleSystem particles;
    private Vector3 otherPlayerVel; // velocity of other player. Used for damage
    private Vector3 thisPlayerVel;  // Velocity of this player. Used for damage
    private GameObject otherPlayer; // another Player's sphere
    //private float speed;
    void Start()
    {

		health = GetComponent<UniversalHealth> ();
		particles = GetComponent<ParticleSystem> ();
		if (gameObject.name == "player1") {
			otherPlayer = GameObject.Find ("player2");
			//otherPlayerVel = otherPlayer.GetComponent<PlayerControl> ().velocity;
            //thisPlayerVel = gameObject.GetComponent<PlayerControl> ().velocity;
		} else if (gameObject.name == "player2") {
			otherPlayer = GameObject.Find ("player1");
			//otherPlayerVel = otherPlayer.GetComponent<PlayerControl> ().velocity;
            //hisPlayerVel = gameObject.GetComponent<PlayerControl> ().velocity;
		}

    }

    void FixedUpdate()
    {
        if (otherPlayer) {
            otherPlayerVel = otherPlayer.GetComponent<PlayerControl> ().velocity;
            thisPlayerVel = gameObject.GetComponent<PlayerControl> ().velocity;
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (otherPlayer && col.gameObject.name == otherPlayer.name) {
            int damage = calculateDamage(col);
            health.damagePlayer(damage);

            particles.Play (); // play collision particle effect
        }
    }
    int calculateDamage(Collision col)
    {
        float damage;
        Vector3 relativePlayerVel = col.relativeVelocity.normalized;
        // get relative position on collision is inpulse
        //print ("this player's vel: " + thisPlayerVel.normalized +"  relative Velocity on collision = " + relativePlayerVel);
        if (relativePlayerVel.x < 0) {relativePlayerVel.x = 0 ;}
        if (relativePlayerVel.y < 0) {relativePlayerVel.y = 0 ;}
        if (relativePlayerVel.z < 0) {relativePlayerVel.z = 0 ;}
        Vector3 collisionAttackValue = (Vector3.Project(col.impulse, relativePlayerVel - thisPlayerVel.normalized).normalized); 

        float attackMagnitude = otherPlayerVel.magnitude;
        damage = (collisionAttackValue.magnitude * attackMagnitude);
        return (int)damage;
    }
}