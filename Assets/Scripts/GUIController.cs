using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    GameManager gameManager;
    PathFinder pathFinder;
    BuildManager buildManager;
    MapManager mapManager;
    
    private ToolTip toolTip;
    private Image selectedIcon;
    private Text selectedDetails;
    private Text livesLabel;
    private Text goldLabel;
    private GameObject gameOverPanel;
    private Tower targettedTower;
    private Image targetIcon;
    private GameObject btnExitEditMode;
    private GameObject txtPathRecalculating;
    private Text txtTileInfo;

    private Button btnTargetFurthest;
    private Button btnTargetClosest;
    private Button btnTargetHighest;
    private Button btnTargetLowest;
    private Button btnTargetRandom;
    private Text txtTargetDescription;
    private GameObject pnlTarget;
    private GameObject pnlStructureInfo;
    private Button btnSellStructure;

    private List<Button> towerTargetButtons;

    [SerializeField]
    private GameObject scrollViewContentBox;

    [SerializeField]
    private List<StructureData> structureDatas;

    //  Used to dynamically create buttons for each type of structure
    //  at runtime
    [SerializeField]
    private GameObject structureBuildBtnPrefab;

    //[SerializeField]
    //private GameObject txtFloatingText;
    [SerializeField]
    GameObject floatingTextGO;
    bool hoveringOverGameObject = false;

    void Start()
    {
        gameManager = GameManager.Instance;
        pathFinder = gameManager.PathFinder;
        buildManager = gameManager.BuildManager;
        mapManager = gameManager.MapManager;

        toolTip = GameObject.Find("ToolTip").GetComponent<ToolTip>();
        selectedIcon = GameObject.Find("imgStructureIcon").GetComponent<Image>();
        selectedDetails = GameObject.Find("txtCurrentlySelectedDetails").GetComponent<Text>();
        livesLabel = GameObject.Find("lblLives").GetComponent<Text>();
        goldLabel = GameObject.Find("lblGold").GetComponent<Text>();
        gameOverPanel = GameObject.Find("pnlGameOver");
        pnlStructureInfo = GameObject.Find("pnlSelectedStructure");
        btnExitEditMode = GameObject.Find("btnExit");
        txtPathRecalculating = GameObject.Find("txtPathRecalculating");
        btnSellStructure = GameObject.Find("btnSellStructure").GetComponent<Button>();
        txtTileInfo = GameObject.Find("txtTileInfo").GetComponent<Text>();

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
        txtPathRecalculating.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitEditMode();
            ClearTarget();
        }

        //  Clear target if user clicks on empty ground
        if (EventSystem.current.IsPointerOverGameObject() == false && 
            hoveringOverGameObject == false && Input.GetMouseButton(0) == true)
        {
           ClearTarget();
        }
    }

    private void OnEnable()
    {
        PathFinder.OnPathRecalculated += HandlePathRecalculated;
        PathFinder.OnPathRecalculating += HandlePathRecalculating;
        MouseManager.OnHoveredNewTile += HandleHoveredNewTile;
        MouseManager.OnHoveredNewGameObject += HandleHoveredNewGameObject;
        MouseManager.OnUnhoveredGameObject += HandleUnhoveredGameObject;
        Enemy.OnEnemyDied += HandleEnemyDied;
        Enemy.OnEnemyHit += HandleEnemyHit;
    }

    private void OnDisable()
    {
        PathFinder.OnPathRecalculated -= HandlePathRecalculated;
        PathFinder.OnPathRecalculating -= HandlePathRecalculating;
        MouseManager.OnHoveredNewTile -= HandleHoveredNewTile;
        MouseManager.OnHoveredNewGameObject -= HandleHoveredNewGameObject;
        MouseManager.OnUnhoveredGameObject -= HandleUnhoveredGameObject;
        Enemy.OnEnemyDied -= HandleEnemyDied;
        Enemy.OnEnemyHit -= HandleEnemyHit;
    }

    /// <summary>
    /// Populates build menu with structure buttons
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

    /// <summary>
    /// Alerts the user that the path is being recalculated
    /// </summary>
    private void HandlePathRecalculating() 
    {
        txtPathRecalculating.SetActive(true);
    }

    /// <summary>
    /// Removes path recalculated message
    /// </summary>
    /// <param name="path"></param>
    /// <param name="index"></param>
    private void HandlePathRecalculated(List<Vector3Int> path) 
    {
        txtPathRecalculating.SetActive(false);
    }

    /// <summary>
    /// Unselects the targetted tower
    /// </summary>
    private void ClearTarget()
    {
        pnlTarget.SetActive(false);
        targettedTower = null;
    }

    /// <summary>
    /// Updates tile info box
    /// </summary>
    /// <param name="tileCoords"></param>
    private void HandleHoveredNewTile(Vector3Int tileCoords)
    {
        if (mapManager != null)
        {
            if (mapManager.ContainsTileAt(MapManager.Layer.PlatformLayer, tileCoords))
            {
                txtTileInfo.text = mapManager.GetTileData(MapManager.Layer.PlatformLayer, tileCoords).ToString();
            }
            else if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, tileCoords))
            {
                txtTileInfo.text = mapManager.GetTileData(MapManager.Layer.GroundLayer, tileCoords).ToString();
            }
            else
            {
                txtTileInfo.text = string.Empty;
            }
        }
    }

    /// <summary>
    /// Shows tooltip if object hovered over implements IDisplayable
    /// </summary>
    /// <param name="gameObject"></param>
    private void HandleHoveredNewGameObject(GameObject gameObject)
    {
        IDisplayable displayable = gameObject.GetComponent<IDisplayable>();

        if (displayable != null)
        {
            toolTip.SetCurrentString(displayable.GetDisplayText());
            ShowToolTip();
        }
        hoveringOverGameObject = true;
    }

    /// <summary>
    /// Hides the tooltip
    /// </summary>
    private void HandleUnhoveredGameObject()
    {
        HideToolTip();
        hoveringOverGameObject = false;
    }

    /// <summary>
    /// Displays floating text indicating an increase in gold
    /// </summary>
    /// <param name="enemy"></param>
    private void HandleEnemyDied(Enemy enemy)
    {
        SpawnFloatingText(enemy.transform.position, string.Format("+{0}g", enemy.EnemyData.Value), Color.yellow);
    }

    /// <summary>
    /// Spawns text showing enemy damage taken
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="damage"></param>
    private void HandleEnemyHit(Enemy enemy, float damage)
    {
        SpawnFloatingText(enemy.transform.position, damage.ToString("F1"), Color.red, 0.25f);
    }

    /// <summary>
    /// Exits the game
    /// </summary>
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

    /// <summary>
    /// Enables the tooltip
    /// </summary>
    public void ShowToolTip()
    {
        toolTip.transform.position = Input.mousePosition;
        toolTip.ShowToolTip();
    }

    /// <summary>
    /// Disables the tooltip
    /// </summary>
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

    /// <summary>
    /// Shows a panel telling the user the game is over
    /// </summary>
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Hides game over panel
    /// </summary>
    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Populate target window with selected tower's info
    /// </summary>
    /// <param name="tower"></param>
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
        float value = Mathf.Round(tower.TowerData.Cost * 0.66f);
        btnSellStructure.GetComponentInChildren<Text>().text = "Sell for " + value.ToString() + " Gold";
        pnlStructureInfo.SetActive(false);
        pnlTarget.SetActive(true);
    }

    /// <summary>
    /// Sells the targetted tower
    /// </summary>
    public void SellTower()
    {
        buildManager.DemolishStructure(Vector3Int.FloorToInt(targettedTower.transform.position));
        pnlTarget.SetActive(false);
    }

    /// <summary>
    /// Clears the highlighted target mode button
    /// </summary>
    public void UnhighlightTowerButtons()
    {
        foreach (Button button in towerTargetButtons)
        {
            button.image.color = Color.white;
        }
    }

    /// <summary>
    /// Tower targets nearest enemy
    /// </summary>
    public void TargetNearest()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Closest);
            btnTargetClosest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets nearest enemy
    /// </summary>
    public void TargetFurthest()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.Furthest);
            btnTargetFurthest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets enemy with lowest health
    /// </summary>
    public void TargetMinHP()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.LowestHealth);
            btnTargetLowest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets enemy with highest health
    /// </summary>
    public void TargetMaxHP()
    {
        if (targettedTower != null)
        {
            UnhighlightTowerButtons();
            targettedTower.SelectTargetMode(Tower.TargetMode.HighestHealth);
            btnTargetHighest.image.color = Color.cyan;
        }
    }

    /// <summary>
    /// Tower targets random enemy within range
    /// </summary>
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

    /// <summary>
    /// Displays a piece of text that floats up and fades away
    /// </summary>
    /// <param name="position"></param>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void SpawnFloatingText(Vector3 position, string message, Color color, float textSize = 0.5f)
    {
        FloatingText floatingText = GameObject.Instantiate(floatingTextGO).GetComponent<FloatingText>();
        floatingText.Initialize(position, message, color, textSize);
    }
    #endregion
}