using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;
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

        for (int i = 0; i < 1; i++)
        {
            offsetx = Random.Range(-0.5f, 0.5f);
            offsety = Random.Range(-0.5f, 0.5f);
            SpawnEnemy(position + new Vector3(offsetx, offsetx));
        }
    }

    /// <summary>
    /// Spawns an enemy at a specified position using the object pool
    /// </summary>
    /// <param name="position"></param>
    private void SpawnEnemy(Vector3 position)
    {
        bool foundAvailible = false;
        //Debug.Log("Existing enemy pool size: " + enemyPool.Count);

        //  Search for an inactive instance in the pool
        foreach (Enemy enemy in enemyPool)
        {
            if (enemy.gameObject.activeInHierarchy == false)
            {
                //Debug.Log("Found an inactive enemy");
                foundAvailible = true;
                enemy.Spawn(position);
                break;
            }
        }

        //  Instantiate a new enemy and add to pool if no inactive enemies exist
        if (foundAvailible == false)
        {
            //Debug.Log("Instantiating a new enemy");
            GameObject go = Instantiate(enemyPrefab, position, new Quaternion());
            Enemy newEnemy = go.GetComponent<Enemy>();
            newEnemy.Spawn(position);
            enemyPool.Add(newEnemy);
        }
    }
}
