using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager: IInitializable, IDisposable {
    IGUIManager guiController;
    IMessageSystem messageSystem;
    IWaveManager waveManager;
    WaveReportPanel waveReportPanel;

    public int Lives { get; private set; }
    public State CurrentState { get; private set; }

    private const int STARTING_LIVES = 25;
    private const float MIN_GAME_SPEED = 1f;
    private const float MAX_GAME_SPEED = 3f;
    private const float GAME_SPEED_INCREMENT = 1f;
    private const int TARGET_FRAMERATE = 60;

    private float currentGameSpeed;

    public GameManager(IGUIManager guiController, IMessageSystem messageSystem, IWaveManager waveManager, WaveReportPanel waveReportPanel) {
        this.guiController = guiController;
        this.messageSystem = messageSystem;
        this.waveManager = waveManager;
        this.waveReportPanel = waveReportPanel;
    }

    public enum State {
        Running,
        Paused,
        Ended,
        Menu
    }

    public void Initialize() {
        Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
        InputHandler.OnCommandEntered += HandlekeyboardInput;
        waveManager.OnWaveStateChanged += HandleWaveStateChanged;
        waveManager.OnPlayerEndedWave += WaveManager_OnPlayerEndedWave;

        Debug.Log("GameManager initializing");
        Lives = STARTING_LIVES;
        guiController.UpdateLivesLabel(Lives);
        currentGameSpeed = MIN_GAME_SPEED;
        SetState(State.Running);
        LimitFramerate();
    }

    public void Dispose() {
        Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
        InputHandler.OnCommandEntered -= HandlekeyboardInput;
        waveManager.OnWaveStateChanged -= HandleWaveStateChanged;
        waveManager.OnPlayerEndedWave -= WaveManager_OnPlayerEndedWave;
    }

    private void HandlekeyboardInput(InputHandler.Command command) {
        switch (command) {
            case InputHandler.Command.TogglePause:
                TogglePause();
                break;
            case InputHandler.Command.DecreaseGameSpeed:
                DecreaseGameSpeed();
                break;
            case InputHandler.Command.IncreaseGameSpeed:
                IncreaseGameSpeed();
                break;
            case InputHandler.Command.ToggleMenu:
                CloseReportPanelOrToggleMenu();
                break;
        }
    }

    private void CloseReportPanelOrToggleMenu() {
        if (waveReportPanel.IsWaveReportOpen()) {
            waveReportPanel.CloseWaveReport();
        }
        else {
            ToggleMenu();
        }
    }

    private void ToggleMenu() {
        if (CurrentState != State.Ended) {
            if (CurrentState == State.Menu) {
                SetState(State.Running);
                guiController.HideMenu();
            }
            else {
                SetState(State.Menu);
                guiController.ShowMenu();
            }
        }
    }

    private void HandleEnemyReachedGate(Enemy enemy) {
        LoseLife();
    }

    public void LoseLife() {
        Lives -= 1;
        guiController.UpdateLivesLabel(Lives);
        messageSystem.DisplayMessage("-1 life", Color.red);
        if (Lives <= 0) {
            GameLost();
        }
    }

    private void WaveManager_OnPlayerEndedWave(object sender, PlayerEndedWaveEventArgs args) {
        LoseLives(args.NumEnemiesRemaining);
    }

    private void LoseLives(int numEnemiesRemaining) {
        Lives -= numEnemiesRemaining;
        guiController.UpdateLivesLabel(Lives);
        messageSystem.DisplayMessage($"-{numEnemiesRemaining} lives", Color.red);
        if (Lives <= 0) {
            GameLost();
        }
    }

    public void GainLife() {
        Lives += 1;
        guiController.UpdateLivesLabel(Lives);
        messageSystem.DisplayMessage("+1 life", Color.green);
        if (Lives > 0) {
            GameContinued();
        }
    }

    private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
        if (arg.newState == IWaveManager.State.WaveInProgress) {
            waveReportPanel.GenerateScoutReport();
            waveReportPanel.CloseWaveReport();
        }
        else if (arg.newState == IWaveManager.State.Waiting) {
            if (Lives > 0) {
                waveReportPanel.ShowWaveReport();
            }
        }
        else if (arg.newState == IWaveManager.State.LastWaveFinished) {
            GameWon();
        }
    }

    private void GameLost() {
        SetState(State.Ended);
        guiController.ShowGameOverScreen();
    }

    private void GameWon() {
        SetState(State.Ended);
        guiController.ShowGameWonScreen();
    }

    private void GameContinued() {
        SetState(State.Running);
        guiController.HideGameEndedPanel();

    }

    /// <summary>
    /// Sets timescale based on game state
    /// </summary>
    /// <param name="state"></param>
    private void SetState(State state) {
        switch (state) {
            case State.Running:
                Time.timeScale = currentGameSpeed;
                guiController.UpdateSpeedPanel(currentGameSpeed);
                guiController.HidePausePanel();
                break;
            case State.Paused:
                Time.timeScale = 0;
                guiController.ShowPausePanel();
                break;
            case State.Ended:
                Time.timeScale = 0;
                break;
            case State.Menu:
                Time.timeScale = 0;
                break;
            default:
                break;
        }

        CurrentState = state;
    }

    public void IncreaseGameSpeed() {
        currentGameSpeed +=  GAME_SPEED_INCREMENT;
        if (currentGameSpeed > MAX_GAME_SPEED) {
            currentGameSpeed = MAX_GAME_SPEED;
        }
        SetState(State.Running);
    }

    public void DecreaseGameSpeed() {
        currentGameSpeed -= GAME_SPEED_INCREMENT;
        if (currentGameSpeed < MIN_GAME_SPEED) {
            currentGameSpeed = MIN_GAME_SPEED;
        }
        SetState(State.Running);
    }

    private void TogglePause() {
        if (CurrentState != State.Ended && CurrentState != State.Menu) {
            SetState(CurrentState == State.Paused ? State.Running : State.Paused);
        }
    }

    private void LimitFramerate() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FRAMERATE;
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevelSelectionScene() {
        SceneManager.LoadScene("LevelSelect");
    }
}
