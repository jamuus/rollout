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

    void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void OnGUI()
    {
        //Draw the health bar for the player
        DrawQuad(new Rect(20, this.name == "player1"? 20 : 80, 200 * (currentHealth/maxHealth), 20), Color.blue);
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
        } else {
            currentHealth += maxHealth;
        }
    }
}
