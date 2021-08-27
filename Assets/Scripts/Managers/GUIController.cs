namespace DefaultNamespace.GUI {

    using DefaultNamespace.IO;
    using System;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public enum GuiState {
        None,
        WaveReport,
        Menu,
        Settings,
        GameEnded
    }

    public class GUIController : IGUIManager, IInitializable, IDisposable {
        private readonly IWaveManager waveManager;
        
        TMP_Text txtLives, txtGold;
        TMP_Text txtCurrentWave, txtTotalWaves, txtWaveCountdown, txtEnemiesRemaining;
        TMP_Text txtGameSpeed, txtFPS;

        GameObject pnlGameEnded, pnlGameOver, pnlGameWon, pnlMenu, pnlPause, pnlWaveReport, pnlSettings;
        GameObject imgBuildMenuLock;

        private GuiState currentState;

        public GUIController(IWaveManager waveManager) {
            Debug.Log("GUIController constuctor");
            this.waveManager = waveManager;
        }

        public void Initialize() {
            GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
            GameManager.OnLivesChanged += GameManager_OnLivesChanged;
            GameManager.OnGameSpeedChanged += GameManager_OnGameSpeedChanged;
            waveManager.OnWaveStateChanged += WaveManager_OnWaveStateChanged;
            waveManager.OnCountDownChanged += WaveManager_OnCountDownChanged;
            waveManager.OnEnemiesRemainingChanged += WaveManager_OnEnemiesRemainingChanged;
            waveManager.OnWaveNumberChanged += WaveManager_OnWaveNumberChanged;
            InputHandler.OnCommandEntered += InputHandler_OnCommandEntered;

            InitializeLabels();
            InitializePanels();

            SetState(GuiState.None);
        }

        public void Dispose() {
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            GameManager.OnLivesChanged -= GameManager_OnLivesChanged;
            GameManager.OnGameSpeedChanged -= GameManager_OnGameSpeedChanged;
            waveManager.OnWaveStateChanged -= WaveManager_OnWaveStateChanged;
            waveManager.OnCountDownChanged -= WaveManager_OnCountDownChanged;
            waveManager.OnEnemiesRemainingChanged -= WaveManager_OnEnemiesRemainingChanged;
            waveManager.OnWaveNumberChanged -= WaveManager_OnWaveNumberChanged;
            InputHandler.OnCommandEntered -= InputHandler_OnCommandEntered;
        }

        private void InitializeLabels() {
            txtLives = GameObject.Find("txtLivesVal").GetComponent<TMP_Text>();
            txtGold = GameObject.Find("txtGoldVal").GetComponent<TMP_Text>();
            txtCurrentWave = GameObject.Find("txtCurrentWaveNum").GetComponent<TMP_Text>();
            txtTotalWaves = GameObject.Find("txtTotalWaveNum").GetComponent<TMP_Text>();
            txtWaveCountdown = GameObject.Find("txtWaveCountdown").GetComponent<TMP_Text>();
            txtEnemiesRemaining = GameObject.Find("txtEnemiesRemaining").GetComponent<TMP_Text>();
            txtGameSpeed = GameObject.Find("txtSpeed").GetComponent<TMP_Text>();
            txtFPS = GameObject.Find("txtFPS").GetComponent<TMP_Text>();
        }

        private void InitializePanels() {
            pnlGameEnded = GameObject.Find("pnlGameEnded");
            pnlGameOver = GameObject.Find("pnlGameOver");
            pnlGameWon = GameObject.Find("pnlGameWon");
            pnlMenu = GameObject.Find("pnlMenu");
            imgBuildMenuLock = GameObject.Find("imgLock");
            pnlPause = GameObject.Find("pnlPause");
            pnlWaveReport = GameObject.Find("pnlWaveReport");
            pnlSettings = GameObject.Find("pnlSettings");

            pnlGameEnded.SetActive(false);
            pnlGameOver.SetActive(false);
            pnlGameWon.SetActive(false);
            pnlMenu.SetActive(false);
            imgBuildMenuLock.SetActive(false);
            pnlPause.SetActive(false);
            pnlSettings.SetActive(false);
        }

        private void GameManager_OnGameStateChanged(object sender, OnGameStateChangedEventArgs e) {
            switch (e.NewState) {
                case GameState.Running:
                    HidePausePanel();
                    break;
                case GameState.Paused:
                    ShowPausePanel();
                    break;
                case GameState.GameLost:
                    HideWaveReportPanel();
                    ShowGameOverScreen();
                    break;
                case GameState.GameWon:
                    HideWaveReportPanel();
                    ShowGameWonScreen();
                    break;
            }
        }

        private void GameManager_OnLivesChanged(object sender, OnLivesChangedEventArgs e) {
            UpdateLivesLabel(e.CurrentLives);
        }

        private void GameManager_OnGameSpeedChanged(object sender, OnGameSpeedChangedEventArgs e) {
            UpdateSpeedPanel(e.GameSpeed);
        }

        private void InputHandler_OnCommandEntered(Command command) {
        if (command == Command.ToggleMenu) {
                if (currentState == GuiState.WaveReport) {
                    HideWaveReportPanel();
                }
                else if (currentState == GuiState.Settings) {
                    HideSettingsPanel();
                }
                else if (currentState == GuiState.Menu) {
                    HideMenu();
                }
                else if (currentState == GuiState.None) {
                    ShowMenu();
                }
            }
        }

        private void WaveManager_OnWaveStateChanged(object sender, WaveStateChangedEventArgs args) {
            if (currentState != GuiState.GameEnded) {
                switch (args.NewState) {
                    case WaveState.Waiting:
                        ShowWaveReportPanel();
                        UnlockBuildMenu();
                        break;
                    case WaveState.WaveInProgress:
                        HideWaveReportPanel();
                        LockBuildMenu();
                        break;
                }
            }
        }

        private void WaveManager_OnEnemiesRemainingChanged(object sender, EnemiesRemainingEventArgs args) {
            UpdateEnemiesRemainingLabel(args.EnemiesRemaining);
        }

        private void WaveManager_OnWaveNumberChanged(object sender, WaveNumberEventArgs args) {
            UpdateWaveNumber(args.CurrentWaveNum, args.MaxWaves);
        }

        private void WaveManager_OnCountDownChanged(object sender, WaveCountdownEventArgs args) {
            UpdateWaveCountdown(args.CountDown);
        }

        private void SetState(GuiState state) {
            currentState = state;
        }

        public void UpdateGoldLabel(float gold) {
            txtGold.text = Mathf.RoundToInt(gold).ToString();
        }

        public void UpdateLivesLabel(int lives) {
            txtLives.text = Mathf.RoundToInt(lives).ToString();
        }

        public void ShowGameOverScreen() {
            pnlMenu.SetActive(false);
            pnlGameEnded.SetActive(true);
            pnlGameOver.SetActive(true);
            pnlGameWon.SetActive(false);
            SetState(GuiState.GameEnded);
        }

        public void ShowGameWonScreen() {
            pnlMenu.SetActive(false);
            pnlGameEnded.SetActive(true);
            pnlGameWon.SetActive(true);
            pnlGameOver.SetActive(false);
            SetState(GuiState.GameEnded);
        }

        public void HideGameEndedPanel() {
            pnlGameWon.SetActive(false);
            pnlGameOver.SetActive(false);
            pnlGameEnded.SetActive(false);
        }

        public void ShowMenu() {
            pnlGameEnded.SetActive(true);
            pnlMenu.SetActive(true);
            SetState(GuiState.Menu);
        }

        public void HideMenu() {
            pnlGameEnded.SetActive(false);
            pnlMenu.SetActive(false);
            SetState(GuiState.None);
        }

        public void ShowWaveReportPanel() {
            pnlWaveReport.SetActive(true);
            SetState(GuiState.WaveReport);
        }

        public void HideWaveReportPanel() {
            pnlWaveReport.SetActive(false);
            SetState(GuiState.None);
        }

        public void ToggleWaveReportPanel() {
            if (pnlWaveReport.activeInHierarchy) {
                HideWaveReportPanel();
            }
            else {
                ShowWaveReportPanel();
            }
        }

        public void ShowSettingsPanel() {
            pnlSettings.SetActive(true);
            SetState(GuiState.Settings);
        }

        public void HideSettingsPanel() {
            pnlSettings.SetActive(false);
            SetState(GuiState.Menu);
        }

        public void UpdateWaveNumber(int current, int total) {
            txtCurrentWave.text = current.ToString();
            txtTotalWaves.text = total.ToString();
        }

        public void UpdateWaveCountdown(int secondsLeft) {
            txtWaveCountdown.text = secondsLeft.ToString();
        }

        public void UpdateSpeedPanel(float speed) {
            txtGameSpeed.text = speed.ToString();
        }

        public void UpdateFPSCounter(int fps) {
            txtFPS.text = fps.ToString();
        }

        public void UpdateEnemiesRemainingLabel(int enemiesRemaining) {
            txtEnemiesRemaining.text = enemiesRemaining.ToString();
        }

        public void LockBuildMenu() {
            imgBuildMenuLock.SetActive(true);
        }

        public void UnlockBuildMenu() {
            imgBuildMenuLock.SetActive(false);
        }

        public void ShowPausePanel() {
            pnlPause.SetActive(true);
        }

        public void HidePausePanel() {
            pnlPause.SetActive(false);
        }
    }
}
