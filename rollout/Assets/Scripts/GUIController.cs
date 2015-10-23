using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour {

    public GameObject player1;
    public GameObject player2;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }

    void OnGUI()
    {
        //Get the health of both players
        int player1Health = player1.GetComponent<UniversalHealth>().currentHealth;
        int player2Health = player2.GetComponent<UniversalHealth>().currentHealth;

        print("Player 1 Health = " + player1Health);

        //Draw the health bar for the players
        DrawQuad(new Rect(20, 20, 200 * ((float)player1Health / UniversalHealth.maxHealth), 20), Color.blue);
        DrawQuad(new Rect(20, 60, 200 * ((float)player2Health / UniversalHealth.maxHealth), 20), Color.red);
    }
}
