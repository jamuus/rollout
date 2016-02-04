using UnityEngine;
using System.Linq;
using System.Collections;

public class GUIController : MonoBehaviour
{

    public GameObject player;
    private LineRenderer lineRenderer;
	private UniversalHealth playerHealth;
	private PlayerControl playerStatus;
	private Status[] statuses = new Status[5];

    // Use this for initialization
    void Start()
    {
		playerHealth = player.GetComponent<UniversalHealth> ();
		playerStatus = player.GetComponent<PlayerControl> ();
		initialiseHealthBar ();
    }

	void initialiseHealthBar()
	{
		lineRenderer = (LineRenderer)GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(2);

		Color playerColor;
		if (player.name == "player1") {
			playerColor = new Color (0.0f, 0.8f, 0.8f);
		} else {
			playerColor = new Color (0.8f, 0.8f, 0.0f);
		}
		lineRenderer.material.color = playerColor;
		statuses = GameObject.Find("Container").GetComponent<InitialiseStatus>().statuses;
	}
    // Update is called once per frame
    void Update()
    {
		renderHealthBar ();
		renderStatuses ();
    }

	void renderHealthBar()
	{
		if (player && player.name == "player1") {
			lineRenderer.SetPosition(0, this.transform.position);
			lineRenderer.SetPosition(1, this.transform.position - new Vector3((12 * getPlayerHealth()), 0,0));

		}
		else if (player && player.name == "player2") {
			lineRenderer.SetPosition(0, this.transform.position);
			lineRenderer.SetPosition(1, this.transform.position + new Vector3((12 * getPlayerHealth()), 0,0));
		}
		else {
			Destroy(gameObject);
		}
		this.GetComponent<TextMesh> ().text = playerHealth.currentHealth.ToString(); //updates value on health bar
	}

    float getPlayerHealth()
    {
        //Work out the proportion
		return (float)playerHealth.currentHealth/(float)playerHealth.getMaxHealth();
    }

	//renders the active statuses
	//two strings are rendered, one indicating inactive, one active statuses
	void renderStatuses()
	{

		int[] playerStatuses = playerStatus.statuses;
		int numberOfStatuses = playerStatuses.Length;
		string activeStatusString = "";
		string inactiveStatusString = "";

		for (int i = 0; i < numberOfStatuses; i++)
		{
			if (playerStatuses [i] > 0) {
				activeStatusString = (activeStatusString + " " + decodeStatusSymbol (i));
				inactiveStatusString = (inactiveStatusString + "    ");
			} else {
				inactiveStatusString = (inactiveStatusString + " " + decodeStatusSymbol (i));
				activeStatusString = (activeStatusString + "    ");
			}
			activeStatusString += " "; //decorative
			inactiveStatusString += " "; //decorative
		}
		GameObject.Find (player.name + "activeStatuses").GetComponent<TextMesh> ().text = activeStatusString;
		GameObject.Find (player.name + "inactiveStatuses").GetComponent<TextMesh> ().text = inactiveStatusString;

	}

	string decodeStatusSymbol(int n)
	{
		string statusName = statuses [n].name;
		string symbol = new string(statusName.Take (3).ToArray());
		return symbol;
	}
}