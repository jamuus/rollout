using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Events : MonoBehaviour {

	public struct GlobalEvent
	{
		public string name;
		public string description;
		public int time; //ms
	}

	public List<GlobalEvent> globalEvents = new List<GlobalEvent>();
	int eventTimer;
	private List<GameObject> spawnedObjects = new List<GameObject>();
	int recurringID = 0; // id = 0 is inactive
    private float timeUntilNextEvent;
    private float timeOfLastEvent;


    public float timeUntilFirstEvent;
    public float timeBetweenEvents;
    public GameObject enemy;
	GameObject player1;
	GameObject player2;


	void Start () {
		initialiseEvents (); // trigger event with id
		player1 = GameObject.Find ("player1");
		player2 = GameObject.Find ("player2");

        timeUntilNextEvent = timeUntilFirstEvent;
        timeOfLastEvent = 0;
	}

	void Update () {
		updateTimer ();
		applyRecurringEvent ();

        if(Time.time > timeOfLastEvent + timeUntilNextEvent)
        {
            //TO-DO: Trigger a vote for the next event
        }
	}
    
	public void initialiseEvents()
	{
		GlobalEvent globalEvent = new GlobalEvent();

		globalEvent.name = "Hell";
		globalEvent.description = "Spawn a lot of lava";
		globalEvent.time = 10000;
		globalEvents.Add (globalEvent);

		globalEvent.name = "Earthquake";
		globalEvent.description = "Shakes spheros up";
		globalEvent.time = 50000;
		globalEvents.Add (globalEvent);

		globalEvent.name = "Enemy";
		globalEvent.description = "Spawn a vicious enemy that shoots at players";
		globalEvent.time = 100000;
		globalEvents.Add (globalEvent);
	}

	public void triggerEvent(int id)
	{
		resetOldState();
		if (id == 0){
			initialiseLava ();
		}
		if (id == 1){
			initialiseEarthquake();
		}
		if (id == 2) {
			initialiseEnemy ();
		}
		setTimer (id);
	}

	void applyRecurringEvent()
	{
		if (recurringID == 1) {
			recurringEarthquake ();
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
		float radius = gameObject.GetComponent<GenerateLevel> ().levelRadius + 2;
		GameObject specialFieldD = gameObject.GetComponent<GenerateLevel> ().specialFieldD;
		for (int i = 0; i < 10; i++)
		{
			Vector3 pos = randomPosition(radius);
			GameObject spawnedObject = (GameObject)Instantiate(specialFieldD, pos, Quaternion.identity);
			spawnedObjects.Add (spawnedObject);
		}
	}

	void initialiseEarthquake()
	{
		recurringID = 1;
	}

	void recurringEarthquake()
	{
		player1.GetComponent<Rigidbody>().AddForce (randomPosition(35), ForceMode.Acceleration);
		player2.GetComponent<Rigidbody>().AddForce (randomPosition(35), ForceMode.Acceleration);
	}

	void initialiseEnemy()
	{
		float radius = gameObject.GetComponent<GenerateLevel> ().levelRadius + 2;
		Vector3 pos = new Vector3 (0f, 1f, 0f);
		GameObject spawnedObject = (GameObject)Instantiate (enemy, pos, Quaternion.identity);
		spawnedObjects.Add (spawnedObject);
	}

	void updateTimer()
	{
		if (eventTimer > 0) {
			eventTimer -= 25;
			if (eventTimer <= 0) {
                timeOfLastEvent = Time.time;
				resetOldState ();
			}
		}
	}

	void setTimer(int id)
	{
		eventTimer = globalEvents [id].time;
	}

	void resetOldState()
	{
		while (spawnedObjects.Count > 0) {
			Destroy (spawnedObjects [0]);
			spawnedObjects.Remove (spawnedObjects[0]);
		}
		recurringID = 0;
	}

}
