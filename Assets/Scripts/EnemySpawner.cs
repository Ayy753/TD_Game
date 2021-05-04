using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameManager gameManager;
    ObjectPool objectPool;
    GameObject entrance;

    //  temp
    [SerializeField]
    List<GameObject> enemyPrefabs;

    void Start()
    {
        gameManager = GameManager.Instance;
        objectPool = gameManager.ObjectPool;
        entrance = GameObject.Find("Entrance");
    }

    /// <summary>
    /// Periodically spawns enemies
    /// </summary>
    /// <returns></returns>
    private IEnumerator PeriodicSpawner()
    {
        while (true)
        {
            SpawnRandomEnemyType(entrance.transform.position);
            yield return new WaitForSeconds(1f);
        }
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
        StartCoroutine(PeriodicSpawner());
    }

    /// <summary>
    /// Stop spawning enemies
    /// </summary>
    public void StopSpawning()
    {
        StopCoroutine(PeriodicSpawner());
    }
}
