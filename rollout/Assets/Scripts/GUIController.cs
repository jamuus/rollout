﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class GUIController : MonoBehaviour
{

    public GameObject player;
    private LineRenderer lineRenderer;
	private UniversalHealth playerHealth;
	private PlayerControl playerStatus;
	private List<Status> statuses = new List<Status>();

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
			drawArc ((float)getPlayerHealth(), -1f);
		}
		else if (player && player.name == "player2") {
			drawArc ((float)getPlayerHealth(), 1f);
		}
		else {
			Destroy(gameObject);
		}
		//this.GetComponent<TextMesh> ().text = playerHealth.currentHealth.ToString(); //updates value on health bar
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

		List<int> playerStatuses = playerStatus.statuses;
		int numberOfStatuses = playerStatuses.Count;
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

	void drawArc(float healthPercentage, float boardSide)
	{
		float theta_scale = 0.01f;
		int size = (int)((1f * 3f) * healthPercentage /theta_scale); //Total number of points in circle.

		lineRenderer = (LineRenderer)GetComponent<LineRenderer>();
		//lineRenderer.SetColors(color, color);
		lineRenderer.SetWidth(1f, 1f);
		lineRenderer.SetVertexCount(size);

		int i = 0; 
		float x, y;
		float r = 12.3f;
		for (float theta = 0; theta < (int)(1f * 3f * healthPercentage); theta += theta_scale) {
			x = (float)(r * Math.Cos (theta * 0.5f));
			y = (float)(r * Math.Sin (theta * 0.5f));

			Vector3 pos = new Vector3 (x * boardSide, 0f, y * boardSide);
			lineRenderer.SetPosition (i, pos);
			i += 1;

		}
	}
}