using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GUIController : MonoBehaviour
{
    GameManager gameManager;
    PathFinder pathFinder;
    BuildManager buildManager;
    MapManager mapManager;
    TowerGUI towerGUI;
    ObjectPool objectPool;
    WaveManager waveManager;

    private ToolTip toolTip;
    private Image selectedIcon;
    private Text selectedDetails;
    private Text livesLabel;
    private Text goldLabel;
    private GameObject gameOverPanel;
    private GameObject btnExitEditMode;
    private GameObject txtPathRecalculating;
    private Text txtTileInfo;
    private GameObject pnlStructureInfo;
    private GameObject txtGamePaused;
    private GameObject pnlWinner;

    private Text currentWaveInfo;
    private Text nextWaveInfo;
    private Text nextWaveCounter;
    private Text waveNumber;
    private TMP_Text txtFps;

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
    GameObject lastHoveredGameObject;

    void Start()
    {
        gameManager = GameManager.Instance;
        pathFinder = gameManager.PathFinder;
        buildManager = gameManager.BuildManager;
        mapManager = gameManager.MapManager;
        towerGUI = gameManager.TowerGUI;
        objectPool = gameManager.ObjectPool;
        waveManager = gameManager.WaveManager;

        toolTip = GameObject.Find("ToolTip").GetComponent<ToolTip>();
        selectedIcon = GameObject.Find("imgStructureIcon").GetComponent<Image>();
        selectedDetails = GameObject.Find("txtCurrentlySelectedDetails").GetComponent<Text>();
        livesLabel = GameObject.Find("lblLives").GetComponent<Text>();
        goldLabel = GameObject.Find("lblGold").GetComponent<Text>();
        gameOverPanel = GameObject.Find("pnlGameOver");
        btnExitEditMode = GameObject.Find("btnExit");
        txtPathRecalculating = GameObject.Find("txtPathRecalculating");
        txtTileInfo = GameObject.Find("txtTileInfo").GetComponent<Text>();
        pnlStructureInfo = GameObject.Find("pnlSelectedStructure");
        txtGamePaused = GameObject.Find("txtGamePaused");
        pnlWinner = GameObject.Find("pnlWinner");

        currentWaveInfo = GameObject.Find("txtCurrentWaveInfo").GetComponent<Text>();
        nextWaveInfo = GameObject.Find("txtNextWaveInfo").GetComponent<Text>();
        nextWaveCounter = GameObject.Find("txtNextWaveCounter").GetComponent<Text>();
        waveNumber = GameObject.Find("txtWaveNum").GetComponent<Text>();
        txtFps = GameObject.Find("txtFps").GetComponent<TMP_Text>();

        HideGameOverPanel();    
        PopulateScrollView();

        btnExitEditMode.SetActive(false);
        txtPathRecalculating.SetActive(false);
        pnlStructureInfo.SetActive(false);
        txtGamePaused.SetActive(false);
        pnlWinner.SetActive(false);

        StartCoroutine(FpsPoller());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitEditMode();
           towerGUI.ClearTarget();
        }

    }

    private void OnEnable()
    {
        PathFinder.OnPathRecalculated += HandlePathRecalculated;
        PathFinder.OnPathRecalculating += HandlePathRecalculating;
        MouseManager.OnHoveredNewTile += HandleHoveredNewTile;
        MouseManager.OnHoveredNewGameObject += HandleHoveredNewGameObject;
        MouseManager.OnUnhoveredGameObject += HandleUnhoveredGameObject;
        MouseManager.OnMouseUp += HandleMouseUp;
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
        MouseManager.OnMouseUp -= HandleMouseUp;
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
            //Debug.Log("created build button");
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
    /// and keeps a reference to the hovered object regardless
    /// </summary>
    /// <param name="gameObject"></param>
    private void HandleHoveredNewGameObject(GameObject gameObject)
    {
        //  If it implements IDisplayable, show tooltip
        IDisplayable displayable = gameObject.GetComponent<IDisplayable>();
        if (displayable != null)
        {
            toolTip.SetCurrentString(displayable.GetDisplayText());
            ShowToolTip();

            //  If this is a tower, show it's attack radius
            Tower tower = gameObject.GetComponent<Tower>();
            if (tower != null)
            {
                TowerData towerData = tower.TowerData;
                buildManager.RenderRadius(gameObject.transform.position, towerData.Range);
            }
        }
        lastHoveredGameObject = gameObject;
    }

    /// <summary>
    /// Hides the tooltip
    /// </summary>
    private void HandleUnhoveredGameObject()
    {
        HideToolTip();

        //  If last hovered object was tower, hide radius
        if (lastHoveredGameObject.GetComponent<Tower>() != null)
        {
            buildManager.HideRadius();
        }

        lastHoveredGameObject = null;
    }

    /// <summary>
    /// Clears the target if user clicks in empty space
    /// or targets tower if user is hovering over it while not in build/demolish mode
    /// </summary>
    private void HandleMouseUp()
    {
        //  If user clicks empty space
        if (lastHoveredGameObject == null)
        {
            //  and cursor isn't over UI
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                towerGUI.ClearTarget();
            }
        }
        //  Otherwise user clicks a game object
        else 
        {
            //  And object is a tower
            Tower tower = lastHoveredGameObject.GetComponent<Tower>();
            if (tower != null)
            {
                //  And build mode is set to none
                if (buildManager.CurrentBuildMode == BuildManager.BuildMode.None)
                {
                    towerGUI.TargetTower(tower);
                }
                //  Otherwise if the user is demolishing tower
                else if (buildManager.CurrentBuildMode == BuildManager.BuildMode.Demolish)
                {
                    HideToolTip();
                }
            }
        }
    }

    /// <summary>
    /// Displays floating text indicating an increase in gold
    /// </summary>
    /// <param name="enemy"></param>
    private void HandleEnemyDied(Enemy enemy)
    {
        SpawnFloatingText(enemy.transform.position, string.Format("+{0}g", enemy.EnemyData.BaseValue), Color.yellow);
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

    private IEnumerator FpsPoller()
    {
        int numSamples = 5;
        int sampleCount = 0;
        float accumulatedFrames = 0;
        float averageFps;
        
        while (true)
        {
            accumulatedFrames += Time.unscaledDeltaTime;
            sampleCount++;

            if (sampleCount == numSamples)
            {
                averageFps = accumulatedFrames / numSamples;
                txtFps.text = "FPS: " + Mathf.RoundToInt(1 / averageFps);
                sampleCount = 0;
                accumulatedFrames = 0;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Restarts the current level
    /// </summary>
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Selects a buildable structure and enters build mode
    /// </summary>
    /// <param name="structureData"></param>
    public void EnterBuildMode(StructureData structureData)
    {
        pnlStructureInfo.SetActive(true);
        btnExitEditMode.SetActive(true);
        selectedIcon.sprite = structureData.Icon;
        selectedDetails.text = structureData.ToString();
        towerGUI.ClearTarget();
        buildManager.EnterBuildMode(structureData);
    }

    /// <summary>
    /// Enters demolish mode
    /// </summary>
    public void EnterDemolishMode()
    {
        pnlStructureInfo.SetActive(false);
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
    /// Shows text showing game is paused
    /// </summary>
    public void ShowPausedText()
    {
        txtGamePaused.SetActive(true);
    }

    /// <summary>
    /// Hides text showing game is paused
    /// </summary>
    public void HidePausedText()
    {
        txtGamePaused.SetActive(false);
    }

    /// <summary>
    /// Shows panel when player wins the game
    /// </summary>
    public void ShowWinnerPanel()
    {
        pnlWinner.SetActive(true);
    }

    public void StartNextWave()
    {
        //  Prevent next wave button from being pressed while game is paused
        if (gameManager.GamePaused != true)
        {
            waveManager.StartSpawning();
        }
    }

    public void UpdateWaveInformation(string currentWave, string nextWave)
    {
        currentWaveInfo.text = currentWave;
        nextWaveInfo.text = nextWave;
    }

    public void UpdateWaveCounter(string waveCounter)
    {
        nextWaveCounter.text = waveCounter;
    }

    public void UpdateWaveNumber(int currentWave, int totalWaves)
    {
        waveNumber.text = string.Format("Wave {0}/{1}", currentWave, totalWaves);
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
        FloatingText floatingText = objectPool.CreateFloatingText();
        floatingText.Initialize(position, message, color, textSize);
    }

    /// <summary>
    /// Spawns floating text at the center of the screen
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    /// <param name="textSize"></param>
    public void SpawnFloatingTextInCenter(string message, Color color, float textSize = 0.5f)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        SpawnFloatingText(pos, message, color, textSize);
    }

    /// <summary>
    /// Spawns floating text at the cursor's position
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    /// <param name="textSize"></param>
    public void SpawnFloatingTextAtCursor(string message, Color color, float textSize = 0.5f)
    {
        SpawnFloatingText(Camera.main.ScreenToWorldPoint(Input.mousePosition), message, color, textSize);
    }
    #endregion
}