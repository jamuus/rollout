using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour
{

    public GameObject player;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = (LineRenderer)GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2);

        Color playerColour = player.name == "player1" ? Color.blue : Color.red;
        lineRenderer.material.color = playerColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (player) {
            //Get the health of both players
            int playerHealth = player.GetComponent<UniversalHealth>().currentHealth;

            lineRenderer.SetPosition(0, player.transform.position + new Vector3(1, 1, -1));
            lineRenderer.SetPosition(1, player.transform.position + new Vector3(1 + (10 * getPlayerHealth(player)), 1, -1));

        } else {
            Destroy(gameObject);
        }
    }

    float getPlayerHealth(GameObject player)
    {
        //Get the absolute health
        float playerHealth = player.GetComponent<UniversalHealth>().currentHealth;

        //Work out the proportion
        return playerHealth / UniversalHealth.maxHealth;
    }
}
