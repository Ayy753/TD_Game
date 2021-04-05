using System.Collections;
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

    private void OnEnable()
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

        StartCoroutine(InitializeGame());
    }

    void Start()
    {
        Debug.Log("GameManager loaded");
    }

    /// <summary>
    /// Used to fix script execution order issues
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(2f);

        EnemySpawner.StartSpawning();
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
            EnemySpawner.StopSpawning();
            Lives = 0;
            GUIController.ShowGameOverPanel();
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
        Gold += enemy.Value;
        print("Gold: " + Gold);
        GUIController.UpdateGameVariableDisplay(Lives, Gold);
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
    private void UnpauseGame()
    {
        Time.timeScale = 1;
    }
}
