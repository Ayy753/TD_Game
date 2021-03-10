using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;
    GameObject entrance;

    void Start()
    {
        entrance = GameObject.Find("Entrance");
        GameObject.Find("Exit").GetComponentInChildren<GateTrigger>().OnEnemyReachedGate += HandleEnemyReachedGate;
    }

    /// <summary>
    /// A testing function that is accessible through unity's inspector
    /// Spawns random enemies at the entrance
    /// </summary>
    [ContextMenu("spawnEnemies")]
    public void SpawnEnemies()
    {
        Vector3 position;
        position = entrance.transform.position;
        float offsetx;
        float offsety;

        for (int i = 0; i < 10; i++)
        {
            offsetx = Random.Range(-1f, 1f);
            offsety = Random.Range(-1f, 1f);
            GameObject.Instantiate(enemyPrefab, position + new Vector3(offsetx, offsety), new Quaternion());
        }
    }
    private void HandleEnemyReachedGate(GameObject enemy)
    {
        Destroy(enemy);
    }
}
