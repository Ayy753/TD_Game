using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;
    GameObject entrance;

    private List<Enemy> enemyPool;

    //  Used to set the speed of all units for testing and demoing
    private float unitSpeedOverride = 0;

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
            SpawnEnemy(position + new Vector3(offsetx, offsetx));
        }
    }
    [ContextMenu("spawnOneEnemy")]
    public void SpawnOneEnemy()
    {
        SpawnEnemy(entrance.transform.position);
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
                enemy.Spawn(position, 20f);
                break;
            }
        }

        //  Instantiate a new enemy and add to pool if no inactive enemies exist
        if (foundAvailible == false)
        {
            //Debug.Log("Instantiating a new enemy");
            GameObject go = Instantiate(enemyPrefab, position, new Quaternion());
            Enemy newEnemy = go.GetComponentInChildren<Enemy>();

            if (unitSpeedOverride != 0)
            {
                newEnemy.SetSpeed(unitSpeedOverride);
            }

            newEnemy.Spawn(position, 20f);
            enemyPool.Add(newEnemy);
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
            SpawnEnemy(entrance.transform.position);
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

    /// <summary>
    /// A method intended for demoing and testing
    /// </summary>
    /// <param name="newSpeed"></param>
    public void ChangeEnemySpeed(float newSpeed)
    {
        unitSpeedOverride = newSpeed;

        foreach (Enemy enemy in enemyPool)
        {
            enemy.SetSpeed(newSpeed);
        }
    }
}
