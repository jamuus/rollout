using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class events : MonoBehaviour {

	public struct GlobalEvent
	{
		public string name;
		public string description;
		public int time; //ms
	}

	public List<GlobalEvent> globalEvents = new List<GlobalEvent>();

	void Start () {
		initialiseEvents (); // trigger event with id

	}

	void Update () {
	}

	void initialiseEvents()
	{
		GlobalEvent globalEvent = new GlobalEvent();

		globalEvent.name = "Hell";
		globalEvent.description = "Spawn a lot of lava";
		globalEvent.time = 10000;
		globalEvents.Add (globalEvent);
	}

	void triggerEvents(int id)
	{
		if (id == 0){
			

		
		}
	}

	Vector3 randomPosition (float radius)
	{
		Vector3 position;
		position = new Vector3((float)Random.Range(-1f,1f), 1f, (float)Random.Range(-1f,1f));
		position = radius * position.normalized;
		position.y = 1f;
		return position;
	}

	void initialiseLava()
	{
		float radius = gameObject.GetComponent<GenerateLevel> ().levelRadius;
		GameObject specialFieldD = gameObject.GetComponent<GenerateLevel> ().specialFieldD;
		for (int i = 0; i < 10; i++)
		{
			Vector3 pos = randomPosition(radius);

			Instantiate(specialFieldD, pos, Quaternion.identity);
		}
	}
}
