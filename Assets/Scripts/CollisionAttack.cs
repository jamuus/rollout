using UnityEngine;
using System.Collections;

public class CollisionAttack : MonoBehaviour
{
    public int collisionAttackValue = 20;
    public UniversalHealth health;
	private float speed;
    void Start()
    {
		health = GetComponent<UniversalHealth> ();
		if (gameObject.name == "player1") {
			speed = GetComponent<PlayerControl> ().speed;
		} else if (gameObject.name == "player2") {
			speed = GetComponent<Player2Control>().speed;
		}
	}
    void OnCollisionEnter(Collision col)
    {
		if (col.gameObject.name == "player1" || col.gameObject.name == "player2")
		{
			GameObject otherPlayer = col.gameObject;
			health.damagePlayer ((int)(collisionAttackValue * otherPlayer.GetComponent<Rigidbody>().velocity.magnitude));
		}
    }
}