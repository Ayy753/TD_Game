using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameManager gameManager;
    ObjectPool objectPool;
    GameObject entrance;
    WaveManager waveManager;
    private int timeBetweenWaves = 5;

    //  temp
    [SerializeField]
    List<GameObject> enemyPrefabs;

    void Start()
    {
        gameManager = GameManager.Instance;
        objectPool = gameManager.ObjectPool;
        waveManager = gameManager.WaveManager;
        entrance = GameObject.Find("Entrance");
    }

    /// <summary>
    /// A function that is accessible through unity's inspector
    /// Spawns random enemies at the entrance
    /// </summary>
    [ContextMenu("spawnEnemies")]
    public void SpawnEnemies()
    {
        Vector3 position = entrance.transform.position;
        float offsetx;
        float offsety;

        for (int i = 0; i < 25 ; i++)
        {
            offsetx = Random.Range(-0.5f, 0.5f);
            offsety = Random.Range(-0.5f, 0.5f);

            SpawnRandomEnemyType(position + new Vector3(offsetx, offsetx));
        }
    }

    /// <summary>
    /// Spawns one random enemy, used for testing
    /// </summary>
    [ContextMenu("spawnOneEnemy")]
    public void SpawnOneEnemy()
    {
        SpawnRandomEnemyType(entrance.transform.position);
    }

    /// <summary>
    /// Spawns a random type of enemy based on a list of prefabs
    /// </summary>
    /// <param name="position"></param>
    public void SpawnRandomEnemyType(Vector3 position)
    {
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        SpawnEnemy(position, enemyPrefab);
    }

    /// <summary>
    /// Spawns an enemy at a specified position using the object pool
    /// </summary>
    /// <param name="position"></param>
    private void SpawnEnemy(Vector3 position, GameObject desiredEnemyPrefab)
    {
        EnemyData desiredEnemyData = desiredEnemyPrefab.GetComponentInChildren<Enemy>().EnemyData;
        Enemy newEnemy = objectPool.CreateEnemy(desiredEnemyData);
        newEnemy.Spawn(position);
    }

    /// <summary>
    /// Start spawning enemies
    /// </summary>
    public void StartSpawning()
    {
        StartCoroutine(StartNextWave());
    }

    /// <summary>
    /// Stop spawning enemies
    /// </summary>
    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    public IEnumerator StartNextWave()
    {
        WaveManager.Wave wave = waveManager.GetNextWave();
        if (wave != null)
        {
            //  Loop through each group of enemies
            foreach (WaveManager.Group group in wave.Groups)
            {
                //  Loop through each enemy in the current group
                for (int i = 0; i < group.NumEnemies; i++)
                {
                    //  Loop through list of prefabs to find one that matches the enemy type
                    foreach (GameObject enemyPrefab in enemyPrefabs)
                    {
                        if (enemyPrefab.name == group.EnemyType)
                        {
                            SpawnEnemy(entrance.transform.position, enemyPrefab);
                        }
                    }
                    //  Wait until next enemy should be spawned
                    yield return new WaitForSeconds(group.TimebetweenSpawns);
                }
                //  Wait until next group should start spawning
                yield return new WaitForSeconds(wave.TimebetweenGroups);
            }
            Debug.Log("wave finished spawning");
            if (waveManager.CurrentWave-1 == wave.WaveNum)
            {
                WaveFinishedSpawning();
            }
        }
    }

    /// <summary>
    /// Counts down and launches next wave when its ready
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaveTimer()
    {
        int secondsUntilNextWave = timeBetweenWaves;

        while (secondsUntilNextWave > 0)
        {
            secondsUntilNextWave--;
            //  todo: update a wave counter via guicontroller
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Next wave started");
        StartCoroutine(StartNextWave());
    }

    private void WaveFinishedSpawning()
    {

        if (waveManager.StillMoreWaves() == true) 
        {
            StopCoroutine(WaveTimer());
            StartCoroutine(WaveTimer());
        }
        else
        {
            Debug.Log("There are no more waves");
        }
    }
}
