using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    GameManager gameManager;
    EnemySpawner spawner;
    PathFinder pathFinder;
    MapManager mapManager;

    [SerializeField]
    private GameObject scrollViewContentBox;

    [SerializeField]
    private List<StructureData> structureDatas;

    [SerializeField]
    private GameObject structureBuildBtnPrefab;

    void Start()
    {
        gameManager = GameManager.Instance;
        spawner = gameManager.EnemySpawner;
        pathFinder = gameManager.PathFinder;
        mapManager = gameManager.MapManager;

        PopulateScrollView();
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void BuildRedTower()
    {
        mapManager.EnterBuildMode(MapManager.StructureClass.RedTower);
    }
    public void BuildBlueTower()
    {
        mapManager.EnterBuildMode(MapManager.StructureClass.BlueTower);
    }
    public void BuildGreenTower()
    {
        mapManager.EnterBuildMode(MapManager.StructureClass.GreenTower);
    }

    public void BuildWall()
    {
        mapManager.EnterBuildMode(MapManager.StructureClass.Wall);
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

    /// <summary>
    /// Programmically adds a build menu button for each type of structure to the scrollview
    /// </summary>
    private void PopulateScrollView()
    {
        print(structureDatas.Count);
        foreach (StructureData structure in structureDatas)
        {
            GameObject newButton = GameObject.Instantiate(structureBuildBtnPrefab);
            newButton.GetComponent<Image>().sprite = structure.icon;
            newButton.name = structure.name;
            newButton.transform.SetParent(scrollViewContentBox.transform);

            print("added a button");
        }
    }
}