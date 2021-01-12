using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float maxEnemies;
    public float spawnTime;
    public float spawnRadius;

    [HideInInspector]
    public float enemyCount = 0;
    private bool canSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCount < maxEnemies && canSpawn) {
            Spawn();
        }
    }

    void Spawn() {
        Vector3 spawnPos = Random.insideUnitCircle * spawnRadius;
        spawnPos = new Vector3(spawnPos.x, 0, spawnPos.y); //lay circle flat on the ground
        spawnPos += transform.position;
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.GetComponent<EnemyHealth>().madeFromSpawner = true;

        canSpawn = false;
        Invoke("rdySpawn", spawnTime);

        enemyCount++;
    }

    void rdySpawn() {
        canSpawn = true;
    }


}
