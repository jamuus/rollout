using UnityEngine;
using System.Collections;

// Field has to have Is Trigger option Enabled on their collider.

// Behaviour Options which specify the behaviour of the fields:
// 0 : damagePlayer
// 1 : healPlayer
// 2 : destroyPlayer

public class SpecialField : MonoBehaviour
{
    //Define field options
	public int behaviourOption = 0;
	public int magnitude = 1;
    public bool isVolatile = false;

	// Use this for initialization
	void Start ()
    {
        renderColor(behaviourOption);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

	void OnTriggerEnter(Collider col)
	{
        //If the field affects the player
		if (col.gameObject.tag == "Player")
        {
            //Get the player object
            GameObject player = col.gameObject;

            //Activate the effect of the field on the player
            switch(behaviourOption)
            {
                case 0:
                    damagePlayer(player);
                    break;

                case 1:
                    healPlayer(player);
                    break;

                case 2:
                    destroyPlayer(player);
                    break;
            }

            //Destroy the special field if it is volatile
            if (isVolatile)
            {
                Destroy(gameObject);
            }
		}
	}

    void renderColor(int option)
    {
        Color color;
        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("UI/Unlit/Transparent");

        switch (behaviourOption)
        {
            case 0:
                color = new Color(.8f, .1f, .1f, .1f); //Red
                break;

            case 1:
                color = new Color(.1f, .8f, .1f, .1f); //Green
                break;

            case 2:
                color = new Color(.8f, .1f, .1f, .4f); //Aggressive Red
                break;

            default:
                color = new Color(0f, 0f, 0f, 0f);
                break;
        }

        //Set the colour
        rend.material.color = color; 
    }

    //Behaviour functions
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

