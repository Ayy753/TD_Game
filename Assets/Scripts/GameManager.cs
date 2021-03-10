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
    public GameObject EntranceGate { get; private set; }
    public GameObject ExitGate { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance = this;
        EntranceGate = GameObject.Find("Entrance");
        ExitGate = GameObject.Find("Exit");
        EnemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
    }

    void Start()
    {
        ExitGate.GetComponentInChildren<GateTrigger>().OnEnemyReachedGate += HandleEnemyReachedGate;
    }

    //  todo: decrease health or something
    private void HandleEnemyReachedGate(GameObject enemy)
    {
        print(enemy.name + " reached the gate");
    }
}
