using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    GameManager gameManager;
    EnemySpawner spawner;
    PathFinder pathFinder;
    MapManager mapManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        spawner = gameManager.EnemySpawner;
        pathFinder = gameManager.PathFinder;
        mapManager = gameManager.MapManager;
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void EnterTowerBuildMode()
    {
        mapManager.EnterBuildMode(MapManager.StructureTile.TowerBase);
    }
    
    public void EnterWallBuildMode()
    {
        mapManager.EnterBuildMode(MapManager.StructureTile.Wall);
    }

    public void EnterDemolishMode()
    {
        mapManager.EnterDemoishMode();
    }

    public void ExitEditMode()
    {
        mapManager.ExitEditMode();
    }

    #region Demo Functions
    /// <summary>
    /// Spawn a unit
    /// </summary>
    public void SpawnEnemy()
    {
        spawner.SpawnEnemy();
    }

    /// <summary>
    /// Change the speed of all units
    /// </summary>
    /// <param name="newSpeed"></param>
    public void ChangeUnitSpeed(float newSpeed)
    {
        spawner.ChangeEnemySpeed(newSpeed);
    }

    /// <summary>
    /// Toggle path highlighting
    /// </summary>
    /// <param name="showPath"></param>
    public void TogglePathVisual(bool showPath)
    {
        if (showPath)
        {
            print("highlighting");
            pathFinder.HighlightPath();
        }
        else
        {
            print("unhighlighting");
            pathFinder.UnhighlightPath();
        }
    }
    #endregion
}