using UnityEngine;
using System.Collections;

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

    public void damagePlayer(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < minHealth) {
            Destroy(this.gameObject);
        }
    }
    public void healPlayer(int healValue)
    {
        if (currentHealth + healValue > maxHealth) {
            currentHealth = maxHealth;
        }
        else {
            currentHealth += healValue;
        }
    }
}
