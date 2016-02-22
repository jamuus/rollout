using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

	GameObject player1;
	GameObject player2;
	public Projectile projectile;

	// Use this for initialization
	void Start () {
		player1 = GameObject.Find("player1");
		player2 = GameObject.Find("player2");
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.Range (0, 24) < 1) {
			shoot ();
		}
	}

	void shoot ()
	{
		Vector3 shotVector = calculateShotVector ();
		shotVector.y = 0;
		Projectile spawnedProjectile = (Projectile)Instantiate(projectile, transform.position + shotVector.normalized, transform.rotation);
		spawnedProjectile.Initialise(shotVector, gameObject);

	}


	GameObject getRandomPlayer()
	{
		int random = (int)Random.Range (0, 2);
		if (random == 1) {
			return player1;
		} else {
			return player2;
		}
	}

	Vector3 calculateShotVector()
	{
		GameObject player = getRandomPlayer ();
		Rigidbody playerRB = player.GetComponent<Rigidbody> ();
		Vector3 playerVelocity = playerRB.velocity;
		Vector3 positionVector = transform.position + player.transform.position *2;
		Vector3 shotVector = (positionVector + playerVelocity);
		return shotVector;
	}
}
