using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject EyeEnemy;
    public GameObject ShipEnemy;
    public GameObject TriEnemy;

    public float firstEyeSpawn;
    public float firstShipSpawn;
    public float firstTriSpawn;

    public float eyeSpawnRate;
    public float shipSpawnRate;
    public float triSpawnRate;

    private float eyeSpawnTimer;
    private float shipSpawnTimer;
    private float triSpawnTimer;

    private bool eyeSpawned;
    private bool shipSpawned;
    private bool triSpawned;

    Transform w;
    Transform e;
    Transform n;
    Transform s;
    Transform[] spawners;

    // Start is called before the first frame update
    void Start()
    {
        w = transform.Find("SpawnerW");
        e = transform.Find("SpawnerE");
        n = transform.Find("SpawnerN");
        s = transform.Find("SpawnerS");
        spawners = new Transform[] { w, e, n, s };
        eyeSpawnTimer = eyeSpawnRate;
        shipSpawnTimer = shipSpawnRate;
        triSpawnTimer = triSpawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (firstEyeSpawn >= 0)
        {
            firstEyeSpawn -= Time.deltaTime;
        }
        else
        {
            if (!eyeSpawned)
            {
                SpawnEye();
                eyeSpawned = true;
            }
            eyeSpawnTimer -= Time.deltaTime;
            if (eyeSpawnTimer <= 0)
            {
                SpawnEye();
            }
        }

        if (firstShipSpawn >= 0)
        {
            firstShipSpawn -= Time.deltaTime;
        }
        else
        {
            if (!shipSpawned)
            {
                SpawnShip();
                shipSpawned = true;
            }
            shipSpawnTimer -= Time.deltaTime;
            if (shipSpawnTimer <= 0)
            {
                SpawnShip();
            }
        }

        if (firstTriSpawn >= 0)
        {
            firstTriSpawn -= Time.deltaTime;
        }
        else
        {
            if (!triSpawned)
            {
                SpawnTri();
                triSpawned = true;
            }
            triSpawnTimer -= Time.deltaTime;
            if (triSpawnTimer <= 0)
            {
                SpawnTri();
            }
        }
    }


    void SpawnEye()
    {
        int i = Random.Range(0, 4);
        Instantiate(EyeEnemy, spawners[i]);
        eyeSpawnTimer = eyeSpawnRate;
    }

    void SpawnShip()
    {
        GameObject go = Instantiate(ShipEnemy, spawners[0]);
        int closest = 0;
        float dist = Vector3.Distance(go.transform.position, spawners[0].position);
        for (int i = 1; i < 4; i++)
        {
            float testDist = Vector3.Distance(go.transform.position, spawners[i].position);
            if (testDist < dist)
            {
                closest = i;
                dist = testDist;
            }
        }
        go.transform.position = spawners[closest].position;
        shipSpawnTimer = shipSpawnRate;
    }

    void SpawnTri()
    {
        GameObject go = Instantiate(TriEnemy, spawners[0]);
        int closest = 0;
        float dist = Vector3.Distance(go.transform.position, spawners[0].position);
        for (int i = 1; i < 4; i++)
        {
            float testDist = Vector3.Distance(go.transform.position, spawners[i].position);
            if (testDist < dist)
            {
                closest = i;
                dist = testDist;
            }
        }
        go.transform.position = spawners[closest].position;
        triSpawnTimer = triSpawnRate;
    }
}
