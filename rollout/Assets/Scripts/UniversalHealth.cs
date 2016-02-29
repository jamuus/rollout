using UnityEngine;
using System.Collections;
using System;

public class UniversalHealth : MonoBehaviour
{
    public static int maxHealth = 100;
    public static int minHealth = 1;
    public int currentHealth = maxHealth;

    // Use this for initialization
    void Start ()
    {
        int currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void updateSphero()
    {
        //Get the sphero
        Sphero sphero = ((PlayerControl)gameObject.GetComponent<PlayerControl>()).sphero;

        //Set properties
        sphero.Health = currentHealth; 
    }

    public void damagePlayer(int damage)
    {
        if (damage > 0)
        {
            currentHealth -= damage;

            //Update the spheros health
            updateSphero();

            //DEBUG
            print(gameObject.name + " takes " + damage + " damage");
        } 

        //Destroy the player if their health drops too low
        if (currentHealth < minHealth)
        {
            Destroy(this.gameObject);
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
