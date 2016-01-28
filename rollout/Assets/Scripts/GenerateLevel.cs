using UnityEngine;
using System.Collections;

public class GenerateLevel : MonoBehaviour
{

    public float levelRadius;
    public int specialFieldN;
    public int upgradeN;
    public int obstacleN;
    public GameObject obstacle;
    public GameObject specialFieldD;
    public GameObject specialFieldH;
    public GameObject upgrade;
    public bool symmetricBattleArena;

    // Use this for initialization
    void Start ()
    {
        initialiseSpecialFields();
        initialiseUpgrades();
        initialiseObstacles();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    Vector3 randomPosition (float radius)
    {
        float randomR = (float)Random.Range(0f, radius);
        Vector3 position;
        if (symmetricBattleArena) {
            position = new Vector3((float)Random.Range(0f, 1f), 1f, (float)Random.Range(-1f, 1f));
        } else {
            position = new Vector3((float)Random.Range(-1f, 1f), 1f, (float)Random.Range(-1f, 1f));
        }
        position = levelRadius * position.normalized;
        position.y = 1f;
        // print(position + "   " + randomR);
        return position;
    }

    void initialiseSpecialFields()
    {
        for (int i = 0; i < specialFieldN; i++) {
            Vector3 pos = randomPosition(levelRadius);
            if (Random.Range(0, 2) == 1) {
                Instantiate(specialFieldD, pos, Quaternion.identity);
                if (symmetricBattleArena) Instantiate(specialFieldD, new Vector3(-pos.x, 1f, pos.z), Quaternion.identity);
            } else {
                Instantiate(specialFieldH, pos, Quaternion.identity);
                if (symmetricBattleArena) Instantiate(specialFieldH, new Vector3(-pos.x, 1f, pos.z), Quaternion.identity);
            }
        }
    }

    void initialiseUpgrades()
    {

        for (int i = 0; i < upgradeN; i++) {
            Vector3 pos = randomPosition(levelRadius);
            Instantiate(upgrade, pos, Quaternion.identity);
            if (symmetricBattleArena) Instantiate(upgrade, new Vector3(-pos.x, 1f, pos.z), Quaternion.identity);
        }
    }

    void initialiseObstacles()
    {

        for (int i = 0; i < obstacleN; i++) {
            Vector3 pos = randomPosition(levelRadius);
            Instantiate(obstacle, pos, Quaternion.identity);
            if (symmetricBattleArena) Instantiate(obstacle, new Vector3(-pos.x, 1f, pos.z), Quaternion.identity);
        }
    }
}
