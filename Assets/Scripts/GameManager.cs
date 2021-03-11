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

    private GameObject exitGate;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance = this;
        exitGate = GameObject.Find("Exit");
        EnemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();

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
        exitGate.GetComponentInChildren<GateTrigger>().OnEnemyReachedGate += HandleEnemyReachedGate;
    }

    //  todo: decrease health or something
    private void HandleEnemyReachedGate(GameObject enemy)
    {
        Debug.Log("Should decrease health");
    }
}
