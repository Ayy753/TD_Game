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

    private GameObject exitGate;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance = this;
        exitGate = GameObject.Find("Exit");
        EnemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        PathFinder = GameObject.Find("PathFinder").GetComponent<PathFinder>();
        GUIController = GameObject.Find("GUIController").GetComponent<GUIController>();

        if (EnemySpawner == null)
        {
            print("spawner is null");
        }
        else if (MapManager == null)
        {
            print("mapmanager is null");
        }
    }

    void Start()
    {
        Debug.Log("GameManager loaded");

    }

    //  todo: decrease health or something
    private void HandleEnemyReachedGate(Enemy enemy)
    {
        Debug.Log("Should decrease health");
    }
}
