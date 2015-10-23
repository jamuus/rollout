using UnityEngine;
using System.Collections;

// used for special filed. Field has to have Is Trigger option Enabled on their collider.
// behaviour options:
// 0 : damagePlayer
// 1 : healPlayer
// 2 : destroyPlayer

public class SpecialField : MonoBehaviour {
	public int behaviourOption = 0; //1 damage health
	public int magnitude = 1; //behaviour's magnitude
    public bool isVolatile = false; // is field destroyed when triggered

	// Use this for initialization
	void Start () {
        renderColor(behaviourOption);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
        // if field affects player
		if (col.gameObject.tag == "Player") {
            GameObject player = col.gameObject;
			if (behaviourOption == 0){
                damagePlayer(player);
			}
            else if (behaviourOption == 1) {
                healPlayer(player);
            }
            else if (behaviourOption == 2) {
                destroyPlayer(player);
            }
            if (isVolatile) {
                Destroy(gameObject);
            }
		}
	}
    void renderColor(int option)
    {
        Color color = new Color(0,0,0,0);
        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("UI/Unlit/Transparent");
        if (behaviourOption == 0) {
            color = new Color(.8f, .1f, .1f, .1f); //red
        }
        else if (behaviourOption == 1)
        {
            color = new Color(.1f, .8f, .1f, .1f); // green
        }
        else if (behaviourOption == 2)
        {
            color = new Color(.8f, .1f, .1f, .4f); // aggressive red
        }
        rend.material.color = color; 
    }

    // behaviour functions
	void damagePlayer(GameObject player)
	{
		UniversalHealth health = player.GetComponent<UniversalHealth> ();
		health.damagePlayer (magnitude);
	}

	void healPlayer(GameObject player)
	{
        UniversalHealth health = player.GetComponent<UniversalHealth> ();
        health.healPlayer (magnitude);
	}

    void destroyPlayer(GameObject player)
    {
        Destroy(player);
    }

}

