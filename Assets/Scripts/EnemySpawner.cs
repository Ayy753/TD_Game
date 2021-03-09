using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;
    GameObject entrance;

    // Start is called before the first frame update
    void Start()
    {
        entrance = GameObject.Find("Entrance");
    }

    /// <summary>
    /// Spawns random enemies at the entrance
    /// </summary>
    [ContextMenu("spawnEnemies")]
    public void SpawnEnemies()
    {
        Vector3 position;
        if (entrance != null)
        {
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
        else
        {
            Debug.LogError("Entrance is null");
        }
    }
}
