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

	void Start ()
	{
	}

	public void Generate ()
	{
		Seed1 ();
	}

	public void Seed1()
	{
		//obstacles
		Coordinates element;
		element.x = 0.0f;
		element.y = 0.6f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 1.8f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 3.0f;
		obstacles.Add (element);

		element.x = 1.2f;
		element.y = 4.2f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 5.4f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 6.6f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 7.8f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 9.0f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 10.2f;
		obstacles.Add (element);

		element.x = 0.0f;
		element.y = 11.4f;
		obstacles.Add (element);

		element.x = -1.2f;
		element.y = 4.2f;
		obstacles.Add (element);



		element.x = -4.6f;
		element.y = 5f;
		obstacles.Add (element);

		element.x = -7.0f;
		element.y = 5f;
		obstacles.Add (element);

		element.x = -5.8f;
		element.y = 6.2f;
		obstacles.Add (element);

		element.x = -5.8f;
		element.y = 3.8f;
		obstacles.Add (element);

		//powerUps
		element.x = 0f;
		element.y = 4.2f;
		powerUps.Add (element);

		element.x = -5.8f;
		element.y = 5f;
		powerUps.Add (element);

		//specialField

		element.x = 4.6f;
		element.y = 5f;
		specialFields.Add (element);

		element.x = 7.0f;
		element.y = 5f;
		specialFields.Add (element);

		element.x = 5.8f;
		element.y = 6.2f;
		specialFields.Add (element);

		element.x = 5.8f;
		element.y = 3.8f;
		specialFields.Add (element);

		initFlag = true;
	}
}
