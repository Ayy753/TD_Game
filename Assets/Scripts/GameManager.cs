using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Acts as the main interface for other scripts to communicate with eachother
/// This script comes first in the execution order
/// and the references to each class are obtained once the OnSceneLoaded event fires
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }
    public MapManager MapManager { get; private set; }
    public PathFinder PathFinder { get; private set; }
    public GUIController GUIController { get; private set; }
    public BuildManager BuildManager { get; private set; }

    public int Lives { get; private set; } = 25;
    public int Gold { get; private set; } = 250;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
        Enemy.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
        Enemy.OnEnemyDied -= HandleEnemyDied;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance = this;
        EnemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        PathFinder = GameObject.Find("PathFinder").GetComponent<PathFinder>();
        GUIController = GameObject.Find("GUIController").GetComponent<GUIController>();
        BuildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();

        if (EnemySpawner == null)
        {
            print("spawner is null");
        }
        else if (MapManager == null)
        {
            print("mapmanager is null");
        }

    }

    private void LateUpdate()
    {
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }

    void Start()
    {
        Debug.Log("GameManager loaded");
    }

    //  todo: decrease health or something
    private void HandleEnemyReachedGate(Enemy enemy)
    {
        Lives -= 1;
        print("Lives: " + Lives);
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }

    private void HandleEnemyDied(Enemy enemy)
    {
        Gold += enemy.Value;
        print("Gold: " + Gold);
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }
}
