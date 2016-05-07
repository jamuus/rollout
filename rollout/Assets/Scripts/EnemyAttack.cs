using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{

    GameObject player1;
    GameObject player2;
    public Projectile projectile;
	private float lastShot = 0f;
	public float rate = 1f;

    // Use this for initialization
    void Start ()
    {
        player1 = GameObject.Find("player1");
        player2 = GameObject.Find("player2");
    }

    // Update is called once per frame
    void Update ()
    {
		if (Time.time > lastShot + rate)
		{
			shoot ();
			lastShot = Time.time;
		}
    }

    void shoot ()
    {
        Vector3 shotVector = calculateShotVector ();
        shotVector.y = 0;
        Projectile spawnedProjectile = (Projectile)Instantiate(projectile, transform.position + 1.5f * shotVector.normalized, transform.rotation);
        spawnedProjectile.Initialise(shotVector);


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
        Vector3 positionVector = transform.position + player.transform.position;
		Vector3 shotVector = (positionVector + playerVelocity * 0.5f * ((positionVector + playerVelocity).magnitude + positionVector.magnitude) /20);
        return shotVector;
    }
}
