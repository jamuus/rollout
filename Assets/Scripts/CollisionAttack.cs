using UnityEngine;
using System.Collections;

public class CollisionAttack : MonoBehaviour
{
    public int collisionAttackValue = 50;
    public UniversalHealth health;
    void Start()
    {
        health = GetComponent<UniversalHealth>();
    }
    void OnCollisionEnter(Collision col)
    {
		if (col.gameObject.name == "player1" || col.gameObject.name == "player2")
		{
			health.damagePlayer (collisionAttackValue);
		}
    }
}