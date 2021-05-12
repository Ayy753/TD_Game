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
    public MapManager MapManager { get; private set; }
    public PathFinder PathFinder { get; private set; }
    public GUIController GUIController { get; private set; }
    public BuildManager BuildManager { get; private set; }
    public TowerGUI TowerGUI { get; private set; }
    public ObjectPool ObjectPool { get; private set; }
    public WaveManager WaveManager { get; private set; }

    public int Lives { get; private set; }
    public int Gold { get; private set; }

    public bool GamePaused { get; private set; } = false;
    //  Flag to prevent user from unpausing while path is recalculating
    private bool pathRecalculating = false;
    private bool gameEnded = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
        Enemy.OnEnemyDied += HandleEnemyDied;
        PathFinder.OnPathRecalculating += HandlePathRecalculating;
        PathFinder.OnPathRecalculated += HandlePathRecalculated;
        WaveManager.OnLastWaveDefeated += HandleLastWaveDefeated;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
        Enemy.OnEnemyDied -= HandleEnemyDied;
        PathFinder.OnPathRecalculating -= HandlePathRecalculating;
        PathFinder.OnPathRecalculated -= HandlePathRecalculated;
        WaveManager.OnLastWaveDefeated -= HandleLastWaveDefeated;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance = this;
        MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        PathFinder = GameObject.Find("PathFinder").GetComponent<PathFinder>();
        GUIController = GameObject.Find("GUIController").GetComponent<GUIController>();
        BuildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();
        TowerGUI = GameObject.Find("TowerGUI").GetComponent<TowerGUI>();
        ObjectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        WaveManager = GameObject.Find("WaveManager").GetComponentInParent<WaveManager>();

        //  Initialize properties
        Lives = 25;
        Gold = 250;

        gameEnded = false;
        StartCoroutine(InitializeGame());
    }

    void Start()
    {
        Debug.Log("GameManager loaded");
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //  Ensure path isnt being recalculated and the game hasnt ended
            if (pathRecalculating == false && gameEnded == false)
            {
                GamePaused = !GamePaused;
                if (GamePaused)
                {
                    PauseGame();
                    GUIController.ShowPausedText();
                }
                else
                {
                    ResumeGame();
                    GUIController.HidePausedText();
                }
            }
        }
    }

    /// <summary>
    /// Used to fix script execution order issues
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(2f);
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }

    /// <summary>
    /// Handles enemy reached gate event
    /// </summary>
    /// <param name="enemy"></param>
    private void HandleEnemyReachedGate(Enemy enemy)
    {
        Lives -= 1;

        if (Lives <= 0 )
        {
            WaveManager.StopSpawning();
            Lives = 0;
            GUIController.ShowGameOverPanel();
            GUIController.HideToolTip();
            PauseGame();
        }

        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }

    /// <summary>
    /// Handles enemy died event
    /// </summary>
    /// <param name="enemy"></param>
    private void HandleEnemyDied(Enemy enemy)
    {
        Gold += enemy.EnemyData.Value;
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }

    /// <summary>
    /// Pauses game when path is recalculating
    /// </summary>
    private void HandlePathRecalculating()
    {
        pathRecalculating = true;
        PauseGame();
    }

    /// <summary>
    /// Resumes game when path is finished calculating
    /// </summary>
    /// <param name="newPath"></param>
    private void HandlePathRecalculated(List<Vector3Int> newPath)
    {
        pathRecalculating = false;
        ResumeGame();
    }

    /// <summary>
    /// End the game when player beats level
    /// </summary>
    private void HandleLastWaveDefeated()
    {
        gameEnded = true;
        GUIController.ShowWinnerPanel();
    }

    /// <summary>
    /// Check if 
    /// </summary>
    /// <param name="price"></param>
    /// <returns></returns>
    public bool CanAfford(int price)
    {
        if (Gold - price >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Spends gold
    /// </summary>
    /// <param name="price"></param>
    public void SpendGold(int price)
    {
        Gold -= price;
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }

    /// <summary>
    /// Adds gold
    /// </summary>
    /// <param name="amount"></param>
    public void GainGold(int amount)
    {
        Gold += amount;
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    private void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
