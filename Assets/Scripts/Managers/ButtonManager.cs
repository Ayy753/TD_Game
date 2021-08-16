using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ButtonManager: IButtonManager, IInitializable {
    protected BuildManager buildManager;
    protected IWaveManager waveManager;
    protected GameManager gameManager;
    protected WaveReportPanel waveReportPanel;

    public ButtonManager(BuildManager buildManager, IWaveManager waveManager, GameManager gameManager, WaveReportPanel waveReportPanel) {
        Debug.Log("ButtonManager constuctor");
        this.buildManager = buildManager;
        this.waveManager = waveManager;
        this.gameManager = gameManager;
        this.waveReportPanel = waveReportPanel;
    }

    public void Initialize() {
        CreateBuildMenuButtons();
        BindButtonsInScene();
    }

    /// <summary>
    /// Populates build menu with structure buttons
    /// </summary>
    public void CreateBuildMenuButtons() {
        GameObject structureBuildBtnPrefab = Resources.Load<GameObject>("Prefabs/NewBuildMenuButton");
        StructureData[] structureDatas = Resources.LoadAll<StructureData>("ScriptableObjects/TileData/StructureData");
        GameObject scrollViewContentBox = GameObject.Find("pnlBuildMenu/Scroll View/Viewport/Content");

        foreach (StructureData structure in structureDatas) {
            if (structure.Buildable == true) {
                GameObject newButton = GameObject.Instantiate(structureBuildBtnPrefab);
                newButton.transform.SetParent(scrollViewContentBox.transform);
                newButton.GetComponent<Image>().sprite = structure.Icon;
                newButton.name = structure.Name;

                //  Not sure why but the scale gets messed up, so this is a fix
                newButton.transform.localScale = new Vector3(1, 1, 1);

                newButton.GetComponent<BuildMenuButton>().Initialize(structure, this);
            }
        }
    }

    public void BindButtonsInScene() {
        Button btnNextWave = GameObject.Find("btnNextWave").GetComponent<Button>();
        Button btnbtnDecreaseSpeed = GameObject.Find("btnDecrease").GetComponent<Button>();
        Button btnIncreaseSpeed = GameObject.Find("btnIncrease").GetComponent<Button>();
        Button btnLevelSelect = GameObject.Find("btnLevelSelect").GetComponent<Button>();
        Button btnRestart = GameObject.Find("btnRestart").GetComponent<Button>();
        Button btnExit = GameObject.Find("btnExit").GetComponent<Button>();
        Button btnDemolish = GameObject.Find("btnDemolish").GetComponent<Button>();
        Button btnToggleWaveReport = GameObject.Find("btnToggleWaveReport").GetComponent<Button>();
        Button btnCloseWaveReport = GameObject.Find("btnCloseWaveReport").GetComponent<Button>();
        Button btnEndWave = GameObject.Find("btnEndWave").GetComponent<Button>();

        btnNextWave.onClick.AddListener(delegate { waveManager.StartNextWave(); });
        btnbtnDecreaseSpeed.onClick.AddListener(delegate { gameManager.DecreaseGameSpeed(); });
        btnIncreaseSpeed.onClick.AddListener(delegate { gameManager.IncreaseGameSpeed(); });
        btnLevelSelect.onClick.AddListener(delegate { gameManager.LoadLevelSelectionScene(); });
        btnRestart.onClick.AddListener(delegate { gameManager.RestartLevel(); });
        btnExit.onClick.AddListener(delegate { gameManager.ExitGame(); });
        btnDemolish.onClick.AddListener(delegate { buildManager.EnterDemolishMode(); });
        btnToggleWaveReport.onClick.AddListener(delegate { waveReportPanel.ToggleWaveReport(); });
        btnCloseWaveReport.onClick.AddListener(delegate { waveReportPanel.CloseWaveReport(); });
        btnEndWave.onClick.AddListener(delegate { waveManager.EndActiveWaves(); });
    }

    public void EnterBuildMode(StructureData structure) {
        buildManager.EnterBuildMode(structure);
    }

    public void EnterDemolishMode() {
        buildManager.EnterDemolishMode();
    }

    public void ExitEditMode() {
        buildManager.ExitBuildOrDemolishMode();
    }

    public void StartNextWave() {
        waveManager.StartNextWave();
    }

    public void Restart() {
        gameManager.RestartLevel();
    }

    public void ExitGame() {
        gameManager.ExitGame();
    }

    public void LevelSelect() {
        gameManager.LoadLevelSelectionScene();
    }

    public void IncreaseaSpeed() {
        gameManager.IncreaseGameSpeed();
    }

    public void DecreaseSpeed() {
        gameManager.DecreaseGameSpeed();
    }
}
