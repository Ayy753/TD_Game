using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    GameManager gameManager;
    EnemySpawner spawner;
    PathFinder pathFinder;
    BuildManager buildManager;
    ToolTip toolTip;

    private Image selectedIcon;
    private Text selectedDetails;

    [SerializeField]
    private GameObject scrollViewContentBox;

    [SerializeField]
    private List<StructureData> structureDatas;

    //  Used to dynamically create buttons for each type of structure
    //  at runtime
    [SerializeField]
    private GameObject structureBuildBtnPrefab;

    void Start()
    {
        gameManager = GameManager.Instance;
        spawner = gameManager.EnemySpawner;
        pathFinder = gameManager.PathFinder;
        buildManager = gameManager.BuildManager;
        selectedIcon = GameObject.Find("imgStructureIcon").GetComponent<Image>();
        selectedDetails = GameObject.Find("txtCurrentlySelectedDetails").GetComponent<Text>();
        toolTip = GameObject.Find("ToolTip").GetComponent<ToolTip>();
        PopulateScrollView();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitEditMode();
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void BuildStructure(StructureData structureData)
    {
        selectedIcon.sprite = structureData.Icon;
        selectedDetails.text = structureData.ToString();
        buildManager.EnterBuildMode(structureData);
    }
    public void EnterDemolishMode()
    {
        selectedIcon.sprite = null;
        selectedDetails.text = string.Empty;
        buildManager.EnterDemolishMode();
    }
    public void ExitEditMode()
    {
        selectedIcon.sprite = null;
        selectedDetails.text = string.Empty;
        buildManager.ExitBuildMode();
    }
    public void SetCurrentTileData(StructureData structureData)
    {
        
    }

    public void ShowToolTip()
    {
        toolTip.ShowToolTip();
    }

    public void HideToolTip() 
    {
        toolTip.HideToolTip();
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
        foreach (StructureData structure in structureDatas)
        {
            GameObject newButton = GameObject.Instantiate(structureBuildBtnPrefab);
            newButton.transform.SetParent(scrollViewContentBox.transform);
            newButton.GetComponent<Image>().sprite = structure.Icon;
            newButton.name = structure.Name;

            //  Not sure why but the scale gets messed up, so this is a fix
            newButton.transform.localScale = new Vector3(1, 1, 1);

            newButton.GetComponent<BuildMenuButton>().SetStructureData(structure);
            Debug.Log("created build button");
        }
    }
}