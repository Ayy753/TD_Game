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
    private GameObject btnExitEditMode;

    private Button btnTargetFurthest;
    private Button btnTargetClosest;
    private Button btnTargetHighest;
    private Button btnTargetLowest;
    private Button btnTargetRandom;
    private Text txtTargetDescription;
    private GameObject pnlTarget;
    private GameObject pnlStructureInfo;

    private List<Button> towerTargetButtons;

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
        pnlStructureInfo = GameObject.Find("pnlSelectedStructure");
        btnExitEditMode = GameObject.Find("btnExit");

        //  Targetting panel UI
        targetIcon = GameObject.Find("imgTargetIcon").GetComponent<Image>();
        btnTargetFurthest = GameObject.Find("btnFurthest").GetComponent<Button>();
        btnTargetClosest = GameObject.Find("btnClosest").GetComponent<Button>();
        btnTargetHighest = GameObject.Find("btnMaxHP").GetComponent<Button>();
        btnTargetLowest = GameObject.Find("btnMinHP").GetComponent<Button>();
        btnTargetRandom = GameObject.Find("btnRandom").GetComponent<Button>();
        txtTargetDescription = GameObject.Find("txtDescription").GetComponent<Text>();
        towerTargetButtons = new List<Button>() { btnTargetFurthest, btnTargetClosest, btnTargetHighest, btnTargetLowest, btnTargetRandom };
        pnlTarget = GameObject.Find("pnlTarget");

        HideGameOverPanel();    
        PopulateScrollView();
        pnlTarget.SetActive(false);
        pnlStructureInfo.SetActive(false);
        btnExitEditMode.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitEditMode();
            ClearTarget();
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

    /// <summary>
    /// Prototype tooltip logic
    /// will be redesigned in future
    /// </summary>
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

            //  If mouse isnt hovering over anything targettable or GUI and mouse button is pressed
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                ClearTarget();
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Selects a buildable structure and enters build mode
    /// </summary>
    /// <param name="structureData"></param>
    public void EnterBuildMode(StructureData structureData)
    {
        pnlStructureInfo.SetActive(true);
        pnlTarget.SetActive(false);
        btnExitEditMode.SetActive(true);
        selectedIcon.sprite = structureData.Icon;
        selectedDetails.text = structureData.ToString();
        buildManager.EnterBuildMode(structureData);
    }

    /// <summary>
    /// Enters demolish mode
    /// </summary>
    public void EnterDemolishMode()
    {
        pnlStructureInfo.SetActive(false);
        pnlTarget.SetActive(false);
        btnExitEditMode.SetActive(true);
        buildManager.EnterDemolishMode();
    }

    /// <summary>
    /// Exits build mode
    /// </summary>
    public void ExitEditMode()
    {
        pnlStructureInfo.SetActive(false);
        btnExitEditMode.SetActive(false);
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

        UnhighlightTowerButtons();

        switch (tower.SelectedTargetMode)
        {
            case Tower.TargetMode.Closest:
                btnTargetClosest.image.color = Color.cyan;
                break;
            case Tower.TargetMode.Furthest:
                btnTargetFurthest.image.color = Color.cyan;
                break;
            case Tower.TargetMode.Random:
                btnTargetRandom.image.color = Color.cyan;
                break;
            case Tower.TargetMode.LowestHealth:
                btnTargetLowest.image.color = Color.cyan;
                break;
            case Tower.TargetMode.HighestHealth:
                btnTargetHighest.image.color = Color.cyan;
                break;
            default:
                throw new Exception("Target mode invalid");
        }

        txtTargetDescription.text = tower.GetDisplayText();
        pnlTarget.SetActive(true);
    }

    private void ClearTarget()
    {
        pnlTarget.SetActive(false);
        targettedTower = null;
    }

    public void UnhighlightTowerButtons()
    {
        foreach (Button button in towerTargetButtons)
        {
            button.image.color = Color.white;
        }
    }

    public void TargetNearest()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Closest);
            btnTargetClosest.image.color = Color.cyan;
        }
    }

    public void TargetFurthest()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Furthest);
            btnTargetFurthest.image.color = Color.cyan;
        }
    }

    public void TargetMinHP()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.LowestHealth);
            btnTargetLowest.image.color = Color.cyan;
        }
    }

    public void TargetMaxHP()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.HighestHealth);
            btnTargetHighest.image.color = Color.cyan;
        }
    }

    public void TargetRandom()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Random);
            btnTargetRandom.image.color = Color.cyan;
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