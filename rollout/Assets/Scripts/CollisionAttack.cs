using UnityEngine;
using System.Collections;
using System;


public class CollisionAttack : MonoBehaviour
{
    //Base attack value
    public int collisionAttackValue = 1;

    //Accessing other parts of the players
    private UniversalHealth health;
    private ParticleSystem particles;
	private GameObject music;

    //Player Velocities
    private Vector3 otherPlayerVel;
    private Vector3 thisPlayerVel;

    //The other player
    private GameObject otherPlayer;

    void Start()
    {
        //Get the other parts of the player
		health = GetComponent<UniversalHealth> ();
		particles = GetComponent<ParticleSystem> ();

		music = gameObject.transform.Find("sound").gameObject;

        //Get the other player
        otherPlayer = GameObject.Find(gameObject.name == "player1" ? "player2" : "player1");
    }

    void FixedUpdate()
    {
        //If the other player object has been set
        if (otherPlayer)
        {
            //Update the players velocities
            otherPlayerVel = otherPlayer.GetComponent<PlayerControl>().velocity;
            thisPlayerVel = gameObject.GetComponent<PlayerControl>().velocity;
        }
    }

    void OnCollisionEnter(Collision col)
    {
		SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
        //If you collided with the other player
		if (otherPlayer && col.gameObject.name == otherPlayer.name || col.gameObject.CompareTag("Shield")) {
			//Calculate and deal some damage to the player
			int damage = calculateDamage (col) * collisionAttackValue;
			health.damagePlayer (damage);
			manager.CollidePlayer (col.relativeVelocity);

			gameObject.GetComponent<PlayerControl>().particles[5].Play();

			//Play collision particle effect
			//particlesmain.Play(); //this causes some errors
		} else if (col.gameObject.tag == "Obstacle") {
			manager.CollideObstacle (col.relativeVelocity);
		} else if (col.gameObject.tag == "Projectile") {
			manager.CollideProjectile (col.relativeVelocity);
		} else if (col.gameObject.tag == "DamageField") {
		}
    }

    int calculateDamage(Collision col)
    {
        Vector3 colImpulse = col.impulse.normalized;

        //Get relative position on collision is impulse
        Vector3 tempThisPlayerVel = thisPlayerVel.normalized;

        //DEBUG
        //print ("this player's vel: " + thisPlayerVel.normalized +"  relative Velocity on collision = " + relativePlayerVel);

        //Calculate the damage to be dealt to the other player
        float collisionAttackValue = Vector3.Dot(otherPlayerVel.normalized, gameObject.transform.position - otherPlayer.transform.position);
        float attackMagnitude = 0.3f * otherPlayerVel.magnitude;
        float damage = (collisionAttackValue * attackMagnitude);

        return (int)damage;
    }
}