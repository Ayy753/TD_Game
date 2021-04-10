using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    GameManager gameManager;
    EnemySpawner spawner;
    PathFinder pathFinder;
    BuildManager buildManager;
    
    private ToolTip toolTip;
    private Image selectedIcon;
    private Text selectedDetails;
    private Text livesLabel;
    private Text goldLabel;
    private GameObject gameOverPanel;
    private Tower targettedTower;
    private Image targetIcon;

    private Button btnTargetFurthest;
    private Button btnTargetClosest;
    private Button btnTargetHighest;
    private Button btnTargetLowest;
    private Button btnTargetRandom;

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

        toolTip = GameObject.Find("ToolTip").GetComponent<ToolTip>();
        selectedIcon = GameObject.Find("imgStructureIcon").GetComponent<Image>();
        selectedDetails = GameObject.Find("txtCurrentlySelectedDetails").GetComponent<Text>();
        livesLabel = GameObject.Find("lblLives").GetComponent<Text>();
        goldLabel = GameObject.Find("lblGold").GetComponent<Text>();
        gameOverPanel = GameObject.Find("pnlGameOver");
        targetIcon = GameObject.Find("targetIcon").GetComponent<Image>();

        btnTargetFurthest = GameObject.Find("btnFurthest").GetComponent<Button>();
        btnTargetClosest = GameObject.Find("btnClosest").GetComponent<Button>();
        btnTargetHighest = GameObject.Find("btnMaxHP").GetComponent<Button>();
        btnTargetLowest = GameObject.Find("btnMinHP").GetComponent<Button>();
        btnTargetRandom = GameObject.Find("btnRandom").GetComponent<Button>();

        HideGameOverPanel();    
        PopulateScrollView();
}

private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitEditMode();
        }

        HandleToolTipLogic();
    }

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

    private void HandleToolTipLogic()
    {
        Vector3 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity);

        IDisplayable display = null;

        //  If physics collider hit a gameobject
        if (hit.collider != null)
        {
            display = hit.collider.GetComponent<IDisplayable>();
        }

        //  Otherwise perform GUI raycast
        else
        {
            PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
            pointerEvent.position = Input.mousePosition;

            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEvent, result);

            //  Check each GUI object hit by ray for an IDisplayable object
            foreach (var item in result)
            {
                display = item.gameObject.GetComponent<IDisplayable>();

                if (display != null)
                {
                    break;
                }
            }
        }

        //  If either rays hit an item with display information, show tooltip
        if (display != null)
        {
            toolTip.ShowToolTip();
            toolTip.SetCurrentString(display.GetDisplayText());
        }
        else
        {
            toolTip.HideToolTip();
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

    public void ShowToolTip()
    {
        toolTip.ShowToolTip();
    }

    public void HideToolTip() 
    {
        toolTip.HideToolTip();
    }

    /// <summary>
    /// Used to update player's gold and lives
    /// </summary>
    /// <param name="lives"></param>
    /// <param name="gold"></param>
    public void UpdateGameVariableDisplay(int lives, int gold)
    {
        livesLabel.text = "Lives: " + lives;
        goldLabel.text = "Gold: " + gold;
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    public void TargetTower(Tower tower)
    {
        targettedTower = tower;
        targetIcon.sprite = targettedTower.TowerData.Icon;

        switch (tower.SelectedTargetMode)
        {
            case Tower.TargetMode.Closest:
                btnTargetClosest.Select();
                break;
            case Tower.TargetMode.Furthest:
                btnTargetFurthest.Select();
                break;
            case Tower.TargetMode.Random:
                btnTargetRandom.Select();
                break;
            case Tower.TargetMode.LowestHealth:
                btnTargetLowest.Select();
                break;
            case Tower.TargetMode.HighestHealth:
                btnTargetHighest.Select();
                break;
            default:
                throw new Exception("Target mode invalid");
        }
    }

    public void TargetNearest()
    {
        if (targettedTower != null)
        {
            targettedTower.SelectTargetMode(Tower.TargetMode.Closest);
            btnTargetClosest.Select();
        }
    }

    public void TargetFurthest()
    {
        if (targettedTower != null)
        {
            targettedTower.SelectTargetMode(Tower.TargetMode.Furthest);
            btnTargetFurthest.Select();
        }
    }

    public void TargetMinHP()
    {
        if (targettedTower != null)
        {
            targettedTower.SelectTargetMode(Tower.TargetMode.LowestHealth);
            btnTargetLowest.Select();
        }
    }

    public void TargetMaxHP()
    {
        if (targettedTower != null)
        {
            targettedTower.SelectTargetMode(Tower.TargetMode.HighestHealth);
            btnTargetHighest.Select();
        }
    }

    public void TargetRandom()
    {
        if (targettedTower != null)
        {
            targettedTower.SelectTargetMode(Tower.TargetMode.Random);
            btnTargetRandom.Select();
        }
    }

    #region Demo Functions
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