using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    GameManager gameManager;
    EnemySpawner spawner;
    PathFinder pathFinder;

    void Start()
    {
        gameManager = GameManager.Instance;
        spawner = gameManager.EnemySpawner;
        pathFinder = gameManager.PathFinder;
    }

    public void SpawnEnemy()
    {
        spawner.SpawnEnemy();
    }
}
