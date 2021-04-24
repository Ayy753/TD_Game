using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemyPrefabs;
    GameObject entrance;

    private List<Enemy> enemyPool;

    void Start()
    {
        entrance = GameObject.Find("Entrance");
        enemyPool = new List<Enemy>();
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
    [ContextMenu("spawnOneEnemy")]
    public void SpawnOneEnemy()
    {
        SpawnRandomEnemyType(entrance.transform.position);
    }

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
        bool foundAvailible = false;
        EnemyData desiredEnemyData = desiredEnemyPrefab.GetComponentInChildren<Enemy>().EnemyData;

        //  Search for an inactive instance in the pool
        foreach (Enemy enemy in enemyPool)
        {
            if (enemy.gameObject.activeInHierarchy == false && enemy.EnemyData == desiredEnemyData)
            {
                foundAvailible = true;
                enemy.Spawn(position);
                break;
            }
        }

        //  Instantiate a new enemy and add to pool if no inactive enemies exist
        if (foundAvailible == false)
        {
            foreach (GameObject enemyPrefab in enemyPrefabs)
            {
                //EnemyData prefabEnemyData = enemyPrefab.GetComponent<Enemy>().EnemyData;
                if (enemyPrefab == desiredEnemyPrefab)
                {
                    GameObject go = Instantiate(enemyPrefab, position, new Quaternion());
                    Enemy newEnemy = go.GetComponentInChildren<Enemy>();
                    newEnemy.Spawn(position);
                    enemyPool.Add(newEnemy);
                    break;
                }
            }
        }
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
