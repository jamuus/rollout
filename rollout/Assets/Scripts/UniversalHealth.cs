using UnityEngine;
using System.Collections;
using System;

public class UniversalHealth : MonoBehaviour
{
    public static int maxHealth = 200;
    public static int minHealth = 1;
    public int currentHealth = 200;
	private GameObject shield;

    // Use this for initialization
    void Start ()
    {
		if (this.gameObject.CompareTag("Player")) {
			maxHealth = currentHealth;
		}
		shield = gameObject.transform.Find("shield").gameObject;
    }

    // Update is called once per frame
    void Update ()
    {
    }

    void updateSphero()
    {
		if (gameObject.CompareTag("Player")) {
        //Get the sphero
        Sphero sphero = ((PlayerControl)gameObject.GetComponent<PlayerControl>()).sphero;

        //Set properties
        	if(sphero != null) sphero.Health = currentHealth;
		}
    }

    public void damagePlayer(int damage)
    {
        print("Health = " + currentHealth + " and min health = " + minHealth);
        if (damage > 0)
        {			
			if (!shield.activeSelf)
				currentHealth -= damage;
			else
				shield.GetComponent<Shield> ().shieldCharge (-damage);

            //Update the spheros health
            updateSphero();

            //DEBUG
            print(gameObject.name + " takes " + damage + " damage");
        }

        //Destroy the player if their health drops too low
		if (currentHealth < minHealth && gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
    public void healPlayer(int healValue)
    {
        //Heal the player clamping the health to the maximum value
        currentHealth = Math.Min(currentHealth + healValue, maxHealth);

        //Update the spheros health
        updateSphero();

    }

    public int getMaxHealth()
	{
		return maxHealth;
	}
}
