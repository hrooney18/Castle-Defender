using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    private float nextSpawnTime;
    private bool spawningWave;
    private float waveLength;
    private float waveTimer;

    private float minSpawnTime;
    private float maxSpawnTime;

    private bool canSpawnRifleman;
    private bool canSpawnRocketman;
    private bool canSpawnTank;
    [SerializeField] private float spawnSpeed;

    public GameObject basicEnemyPrefab;
    public GameObject riflemanPrefab;
    public GameObject rocketmanPrefab;
    public GameObject tankPrefab;

    private GameManager gm;
    private bool shopOpen;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        nextSpawnTime = 0;
        minSpawnTime = 1f;
        maxSpawnTime = 1.5f;
        spawnSpeed = Random.Range(minSpawnTime, maxSpawnTime);
        spawningWave = false;
        waveLength = 25;
        waveTimer = waveLength;

        canSpawnRifleman = false;
        canSpawnRocketman = false;
        canSpawnTank = false;
        shopOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawningWave)
        {
            if (waveTimer > 0)
                SpawnWave();
            else
                spawningWave = false;
            waveTimer -= Time.deltaTime;
        } else if (!shopOpen && !gm.startOfGame)
        {
            int childCount = gm.enemies.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(gm.enemies.transform.GetChild(i).gameObject);
            }

            waveTimer = waveLength;
            gm.OpenShop();
            shopOpen = true;
        }
    }

    void SpawnWave()
    {
        if (Time.time >= nextSpawnTime)
        {
            if (canSpawnRifleman)
            {
                int rand1 = Random.Range(0, 5);
                if (rand1 == 0)
                {
                    SpawnEnemy("Rifleman");
                    return;
                }

                if (canSpawnRocketman)
                {
                    int rand2 = Random.Range(0, 5);
                    if (rand2 == 0)
                    {
                        SpawnEnemy("Rocketman");
                        return;
                    }
                    if (canSpawnTank)
                    {
                        int rand3 = Random.Range(0, 5);
                        if (rand3 == 0)
                        {
                            SpawnEnemy("Tank");
                            return;
                        }

                        SpawnEnemy("Basic");
                        return;
                    }

                    SpawnEnemy("Basic");
                    return;
                }

                SpawnEnemy("Basic");
                return;
            }

            SpawnEnemy("Basic");
            return;
        }
    }

    private void SpawnEnemy(string enemyType)
    {
        GameObject enemy;
        switch (enemyType)
        {
            case "Basic":
                enemy = Instantiate(basicEnemyPrefab, new Vector3(-12.5f, Random.Range(-5.8f, -3), 0), Quaternion.identity) as GameObject;
                enemy.transform.SetParent(gm.enemies.transform);
                break;
            case "Rifleman":
                enemy = Instantiate(riflemanPrefab, new Vector3(-12.5f, Random.Range(-5.8f, -3), 0), Quaternion.identity) as GameObject;
                enemy.transform.SetParent(gm.enemies.transform);
                break;
            case "Rocketman":
                enemy = Instantiate(rocketmanPrefab, new Vector3(-12.5f, Random.Range(-5.8f, -3), 0), Quaternion.identity) as GameObject;
                enemy.transform.SetParent(gm.enemies.transform);
                break;
            case "Tank":
                enemy = Instantiate(tankPrefab, new Vector3(-12.5f, Random.Range(-5.8f, -3), 0), Quaternion.identity) as GameObject;
                enemy.transform.SetParent(gm.enemies.transform);
                break;
            default:
                break;
        }

        spawnSpeed = Random.Range(minSpawnTime, maxSpawnTime);
        nextSpawnTime = Time.time + spawnSpeed;
    }

    public void AssignWave()
    {
        int wave = gm.wave;
        if (wave == 6)
            canSpawnRifleman = true;
        if (wave == 12)
            canSpawnRocketman = true;
        if (wave == 18)
            canSpawnTank = true;

        if (wave != 1)
        {
            minSpawnTime -= (wave / 100);
            maxSpawnTime -= (wave / 100);
        }

        minSpawnTime = Mathf.Clamp(minSpawnTime, 0, 1);
        maxSpawnTime = Mathf.Clamp(maxSpawnTime, 0, 1.5f);

        spawningWave = true;
        shopOpen = false;
    }
}
