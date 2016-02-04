using UnityEngine;
using System.Collections;
using System.Linq;

public class GenerateLevel : MonoBehaviour {

    public float levelRadius;
    public int specialFieldN;
    public int upgradeN;
    public int obstacleN;
    public GameObject obstacle;
    public GameObject specialFieldD;
    public GameObject specialFieldH;
    public GameObject upgrade;
	public bool random;
    public bool symmetricBattleArena;
	private LevelSeed seed;

	// Use this for initialization
	void Start () {
		
		if (random) {
			initialiseSpecialFields ();
			initialiseUpgrades ();
			initialiseObstacles ();
		} else {
			print ("getting seed");
			seed = GameObject.Find("Container").GetComponent<LevelSeed> ();
			seed.Generate ();
			print ("got seed");
			spawnSeed(seed);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

    Vector3 randomPosition (float radius)
    {
        float randomR = (float)Random.Range(0f, radius);
        Vector3 position;
        if (symmetricBattleArena)
        {
            position = new Vector3((float)Random.Range(0f,1f), 1f, (float)Random.Range(-1f,1f));
        }
        else 
        {
            position = new Vector3((float)Random.Range(-1f,1f), 1f, (float)Random.Range(-1f,1f));
        }
        position = levelRadius * position.normalized;
        position.y = 1f;
        print(position + "   " + randomR);
        return position;
    }
    
    void initialiseSpecialFields()
    {
        for (int i = 0; i < specialFieldN; i++)
        {
            Vector3 pos = randomPosition(levelRadius);
            if(Random.Range(0,2) == 1)
            {
                Instantiate(specialFieldD, pos, Quaternion.identity);
                if (symmetricBattleArena) Instantiate(specialFieldD, new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
            }
            else
            {
                Instantiate(specialFieldH, pos, Quaternion.identity);
                if (symmetricBattleArena) Instantiate(specialFieldH, new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
            }
        }
    }

    void initialiseUpgrades()
    {

		for (int i = 0; i < upgradeN; i++) {
			Vector3 pos = randomPosition (levelRadius);
			GameObject powerUp = (GameObject)Instantiate (upgrade, pos, Quaternion.identity);
			powerUp.GetComponent<SpecialField> ().setPowerUpID ((int)Random.Range (0f, 5f));
			if (symmetricBattleArena) {
				powerUp = (GameObject)Instantiate (upgrade, new Vector3 (-pos.x, 1f, -pos.z), Quaternion.identity);
				powerUp.GetComponent<SpecialField>().setPowerUpID((int)Random.Range(0f,5f));
			}
		}
    }

    void initialiseObstacles()
    {

        for (int i = 0; i < obstacleN; i++)
        {
            Vector3 pos = randomPosition(levelRadius);
            Instantiate(obstacle, pos, Quaternion.identity);
            if (symmetricBattleArena) Instantiate(obstacle,new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
        }
    }

	void spawnSeed(LevelSeed seed)
	{
		print ("" + seed.obstacles.Count + "   " + seed.powerUps.Count + "   " + seed.specialFields.Count + "   ");
		for (int i = 0; i < seed.obstacles.Count (); i++)
		{
			Vector3 pos = new Vector3 (seed.obstacles [i].x, 1f, seed.obstacles [i].y);
			Instantiate(obstacle, pos, Quaternion.identity);
			if (symmetricBattleArena) Instantiate(obstacle,new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
		}

		for (int i = 0; i < seed.powerUps.Count (); i++) {
			Vector3 pos = new Vector3 (seed.powerUps [i].x, 1f, seed.powerUps [i].y);
			GameObject powerUp = (GameObject)Instantiate (upgrade, pos, Quaternion.identity);
			powerUp.GetComponent<SpecialField> ().setPowerUpID ((int)Random.Range (0f, 5f));

			if (symmetricBattleArena) {
				powerUp = (GameObject)Instantiate (upgrade, new Vector3 (-pos.x, 1f, -pos.z), Quaternion.identity);
				powerUp.GetComponent<SpecialField>().setPowerUpID((int)Random.Range(0f,5f));
			}
		}

		for (int i = 0; i < seed.specialFields.Count(); i++)
		{
			Vector3 pos = new Vector3 (seed.specialFields [i].x, 1f, seed.specialFields [i].y);
			Instantiate(specialFieldD, pos, Quaternion.identity);
			if (symmetricBattleArena) Instantiate(specialFieldD,new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
		}
	}
}