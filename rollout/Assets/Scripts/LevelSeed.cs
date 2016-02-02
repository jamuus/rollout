using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public struct Coordinates
{
	public float x;
	public float y;
}

public class LevelSeed: MonoBehaviour
{
	public List<Coordinates> obstacles = new List<Coordinates>();
	public List<Coordinates> powerUps = new List<Coordinates>();
	public List<Coordinates> specialFields = new List<Coordinates>();
	public bool initFlag = false;

	void Start(){
		//obstacles
		Coordinates element;
		element.x = 0f;
		element.y = 1f;
		obstacles.Add (element);

		element.x = 0f;
		element.y = 6f;
		obstacles.Add (element);

		element.x = 6f;
		element.y = 2f;
		obstacles.Add (element);

		element.x = 6f;
		element.y = 0f;
		obstacles.Add (element);

		//powerUps
		element.x = 0f;
		element.y = 3f;
		powerUps.Add (element);

		element.x = 4f;
		element.y = 0f;
		powerUps.Add (element);
		/*
		element.x = 7f;
		element.y = -4;
		powerUps.Add (element);

		element.x = 6f;
		element.y = -1f;
		powerUps.Add (element);
		*/
		//specialField

		element.x = 3f;
		element.y = -3f;
		specialFields.Add (element);
		print (obstacles [3].x + " and " + obstacles[3].y + "   is the 4th obstacle");
		initFlag = true;
	}
}
