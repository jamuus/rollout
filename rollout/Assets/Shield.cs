using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    public GameObject player;
    public int health;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void shieldCharge(int amount)
    {
        health += amount;

        //If the shield runs out of health, deactiveate it
        this.gameObject.SetActive(health > 0);
    }

    void LateUpdate()
    {
        if (transform.rotation != Quaternion.Euler(0, 0, 0))
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    int getImpactDamage(Collision collision)
    {
        GameObject collidingObject = collision.gameObject;

        if (collidingObject.CompareTag("Projectile")) return collidingObject.GetComponent<Projectile>().damage;
        else return 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        //If you hit another player move them
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Projectile"))
        {
            //Get the force
            Vector3 force = collision.relativeVelocity / 2;

            //Do damage to the shield based on magnitude 
            shieldCharge(-getImpactDamage(collision));

            //Move the player
            if (collision.gameObject.CompareTag("Player")) player.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
        }

        //Only collide if its a weapon
        else if (!collision.gameObject.CompareTag("Projectile") && !collision.gameObject.CompareTag("Shield"))
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());

    }
}