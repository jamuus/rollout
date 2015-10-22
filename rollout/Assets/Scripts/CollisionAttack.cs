using UnityEngine;
using System.Collections;

public class CollisionAttack : MonoBehaviour
{
    public int collisionAttackValue = 10; //Base attack value
    private UniversalHealth health; // Access to UniversalHealth class
    private ParticleSystem particles;
    private Vector3 otherPlayerVel; // velocity of other player. Used for damage
    private GameObject otherPlayer; // another Player's sphere
    //private float speed;
    void Start()
    {
        health = GetComponent<UniversalHealth> ();
        particles = GetComponent<ParticleSystem> ();
        if (gameObject.name == "player1") {
            otherPlayer = GameObject.FindWithTag ("Player2");
            otherPlayerVel = otherPlayer.GetComponent<PlayerControl> ().velocity;
        } else if (gameObject.name == "player2") {
            otherPlayer = GameObject.FindWithTag ("Player1");
            otherPlayerVel = otherPlayer.GetComponent<PlayerControl> ().velocity;
        }

    }

    void FixedUpdate()
    {
        if (otherPlayer) {
            otherPlayerVel = otherPlayer.GetComponent<PlayerControl> ().velocity;
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (otherPlayer && col.gameObject.name == otherPlayer.name) {
            float attackMagnitude = otherPlayerVel.magnitude;
            int damage = (int)(collisionAttackValue * attackMagnitude);
            health.damagePlayer(damage);
            print(otherPlayer.name + " deals " + damage + " damage");
            particles.Play ();
        }
    }
}