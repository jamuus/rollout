using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GenerateLevel : MonoBehaviour
{
    public int boundaryHardness;
    public float levelRadius;
    public int specialFieldN;
    public int upgradeN;
    public int obstacleN;
    public int clustersN;
    public int clusterSize;
    public GameObject obstacle;
    public GameObject smallObstacle;
    public GameObject largeObstacle;
    public GameObject specialFieldD;
    public GameObject specialFieldH;
    public GameObject upgrade;
    public bool random;
    public bool symmetricBattleArena;
    public bool clusterObstacles;
    private LevelSeed seed;
    private List<GameObject> placedObstacles = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        scaleScene();

        if (random) {
            initialiseSpecialFields ();
            initialiseUpgrades ();
            if (clusterObstacles) initialiseClusters();
            else initialiseObstacles ();
        } else {
            print ("getting seed");
            seed = GameObject.Find("Container").GetComponent<LevelSeed> ();
            seed.Generate ();
            print ("got seed");
            spawnSeed(seed);
        }
    }

    void scaleScene()
    {
        //Set the size of the arena
        GameObject circularPlane = GameObject.Find("Circular Plane");
        if (circularPlane != null)
            circularPlane.GetComponent<Transform>().localScale = new Vector3(levelRadius * 100, 1, levelRadius * 100);

        //Set the initial player positions
        GameObject.Find("player1").GetComponent<Transform>().position = new Vector3(-(levelRadius - 4), 1, 0);
        GameObject.Find("player2").GetComponent<Transform>().position = new Vector3((levelRadius - 4), 1, 0);

        //Set the position of the text status effects
        GameObject.Find("player1Health").GetComponent<Transform>().position = new Vector3(-0.2f, 1, levelRadius+1);
        GameObject.Find("player2Health").GetComponent<Transform>().position = new Vector3(0.2f, 1, levelRadius+1);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void addObject(GameObject block, Vector3 position)
    {
        //Check that the position is free
        Collider[] blockColliders = Physics.OverlapSphere(position, ((Collider)block.GetComponent<Collider>()).bounds.extents.magnitude * 1.2f);
        foreach (Collider collider in blockColliders) {
            if (collider.name.Contains("Obstacle")) {
                print("COLLISION: " + collider.name);
                return;
            }
        }


        print("Placed " + block.name + " at " + block.GetComponent<Renderer>().bounds.ToString());

        //Add the block
        block.transform.position = position;
        Instantiate(block, position, Quaternion.identity);
        placedObstacles.Add(block);
    }

    Vector3 randomPosition (float radius)
    {
        Vector3 position;

        if (symmetricBattleArena) {
            position = new Vector3((float)Random.Range(0f, 1f), 1f, (float)Random.Range(-1f, 1f));
        } else {
            position = new Vector3((float)Random.Range(-1f, 1f), 1f, (float)Random.Range(-1f, 1f));
        }
        position = radius * position.normalized;
        position.y = 1f;

        return position;
    }

    void initialiseSpecialFields()
    {
        for (int i = 0; i < specialFieldN; i++) {
            Vector3 pos = randomPosition(levelRadius);
            if (Random.Range(0, 2) == 1) {
                Instantiate(specialFieldD, pos, Quaternion.identity);

                if (symmetricBattleArena) Instantiate(specialFieldD, new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
            } else {
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
                powerUp.GetComponent<SpecialField>().setPowerUpID((int)Random.Range(0f, 5f));
            }
        }
    }


    void initialiseClusters()
    {
        //For all clusters
        for (int i = 0; i < clustersN; i++) {
            //Generate a root position
            Vector3 clusterCentre = randomPosition(levelRadius * 0.9f);

            //Generate the cluster
            initialiseCluster(clusterCentre);

            //If the centre of the cluster is far from the boundary and symmetry is on
            if (clusterCentre.x > 0.2 && symmetricBattleArena) {
                //Generate a second cluster on the other side
                initialiseCluster(new Vector3(-clusterCentre.x, 1, -clusterCentre.z));

                //Count the extra cluster
                i++;
            }
        }
    }

    bool inBounds(Vector3 point)
    {
        if (point.x < -levelRadius * 0.7 || point.x > levelRadius * 0.7) return false;
        if (point.z < -levelRadius * 0.7 || point.z > levelRadius * 0.7) return false;
        return true;
    }

    Vector3 findFreeSide(List<GameObject> blocks)
    {
        //Find where to place the next block
        bool freeSideFound = false;
        int attempts = 0;
        while (!freeSideFound && attempts < blocks.Count * 1.4) {
            //Get a random block in the list
            GameObject nextNeighbour = blocks.ElementAt(Random.Range(0, blocks.Count - 1));

            //Look around it to try and find a free side
            for (int j = 0; j < 4; j++) {
                //Generate a direction
                Vector3 neighbourDirection = (j == 0) ? new Vector3(1, 0, 0).normalized : (j == 1) ? new Vector3(0, 0, 1).normalized : (j == 2) ? new Vector3(-1, 0, 0).normalized : new Vector3(0, 0, -1).normalized;


                //Check if that side is free
                bool sideFree = true;
                foreach (GameObject block in blocks) {
                    if (block != nextNeighbour && ((block.transform.position - nextNeighbour.transform.position).normalized - neighbourDirection).magnitude < 0.2)
                        sideFree = false;
                }

                //Check if the block placed on this side would be in bounds
                neighbourDirection.Scale(nextNeighbour.transform.localScale * 1.0f);
                if (!inBounds(nextNeighbour.transform.position + neighbourDirection)) sideFree = false;

                //If the side is free
                if (sideFree) {
                    //Return the point on the free edge of the next neighbour
                    return nextNeighbour.transform.position + neighbourDirection;
                }
            }

            attempts++;
        }

        return Vector3.zero;
    }

    void initialiseCluster(Vector3 centre)
    {
        //Set the position
        Vector3 blockPosition = centre;

        //Define the list of already placed blocks
        List<GameObject> placedBlocks = new List<GameObject>();

        //Generate each cluster block
        for (int i = 1; i <= clusterSize; i++) {
            //Set the block size
            float ratio = (float)i / clusterSize;
            GameObject block = ratio < 0.33 ? largeObstacle : ratio < 0.66 ? obstacle : smallObstacle;

            //Instantiate the block
            placedBlocks.Add(block);
            addObject(block, blockPosition);

            //Update the position
            Vector3 modifier = findFreeSide(placedBlocks);
            if (modifier.magnitude > 0) blockPosition = modifier;

            else return;
        }
    }

    void initialiseObstacles()
    {
        for (int i = 0; i < obstacleN; i++) {
            //Calculate the new position
            Vector3 pos = randomPosition(levelRadius);

            addObject(obstacle, pos);

            //If we want a symmetrical arena
            if (symmetricBattleArena) addObject(obstacle, new Vector3(-pos.x, 1f, -pos.z));
        }
    }

    void spawnSeed(LevelSeed seed)
    {
        print ("" + seed.obstacles.Count + "   " + seed.powerUps.Count + "   " + seed.specialFields.Count + "   ");
        for (int i = 0; i < seed.obstacles.Count (); i++) {
            Vector3 pos = new Vector3 (seed.obstacles [i].x, 1f, seed.obstacles [i].y);
            Instantiate(obstacle, pos, Quaternion.identity);
            if (symmetricBattleArena) Instantiate(obstacle, new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
        }

        for (int i = 0; i < seed.powerUps.Count (); i++) {
            Vector3 pos = new Vector3 (seed.powerUps [i].x, 1f, seed.powerUps [i].y);
            GameObject powerUp = (GameObject)Instantiate (upgrade, pos, Quaternion.identity);
            powerUp.GetComponent<SpecialField> ().setPowerUpID ((int)Random.Range (0f, 5f));

            if (symmetricBattleArena) {
                powerUp = (GameObject)Instantiate (upgrade, new Vector3 (-pos.x, 1f, -pos.z), Quaternion.identity);
                powerUp.GetComponent<SpecialField>().setPowerUpID((int)Random.Range(0f, 5f));
            }
        }

        for (int i = 0; i < seed.specialFields.Count(); i++) {
            Vector3 pos = new Vector3 (seed.specialFields [i].x, 1f, seed.specialFields [i].y);
            Instantiate(specialFieldD, pos, Quaternion.identity);
            if (symmetricBattleArena) Instantiate(specialFieldD, new Vector3(-pos.x, 1f, -pos.z), Quaternion.identity);
        }
    }

	public void restart()
	{
		GameObject[] generatedObjects = GameObject.FindGameObjectsWithTag ("Generated");
		foreach (GameObject obj in generatedObjects) {
			Destroy (obj);
		}
		placedObstacles.Clear ();
		Start ();
	}
}