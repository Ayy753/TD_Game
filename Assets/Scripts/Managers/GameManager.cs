using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager: IInitializable, IDisposable {
    IGUIManager guiController;
    IMessageSystem messageSystem;
    public int Lives { get; private set; }

    public State CurrentState { get; private set; }

    private const int startingLives = 25;

    private const float minSpeed = 1f;
    private const float maxSpeed = 3f;
    private const float speedIncrement = 1f;
    private float currentGameSpeed;

    private const int targetFrameRate = 60;

    public GameManager(IGUIManager guiController, IMessageSystem messageSystem) {
        this.guiController = guiController;
        this.messageSystem = messageSystem;
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
        WaveManager.OnStateChanged += HandleWaveStateChanged;
        
        Debug.Log("GameManager initializing");
        Lives = startingLives;
        guiController.UpdateLivesLabel(Lives);
        currentGameSpeed = minSpeed;
        SetState(State.Running);
        LimitFramerate();
    }

    public void Dispose() {
        Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
        InputHandler.OnCommandEntered -= HandlekeyboardInput;
        WaveManager.OnStateChanged -= HandleWaveStateChanged;
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
                ToggleMenu();
                break;
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
        Lives -= 1;
        guiController.UpdateLivesLabel(Lives);
        messageSystem.DisplayMessage("-1 life", Color.red);
        if (Lives <= 0) {
            guiController.ShowGameOverScreen();
            SetState(State.Ended);
        }
    }

    private void HandleWaveStateChanged(WaveManager.State newState) {
        if (newState == WaveManager.State.LastWaveFinished) {
            guiController.ShowGameWonScreen();
            SetState(State.Ended);
        }
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
                break;
            case State.Paused:
                Time.timeScale = 0;
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
        currentGameSpeed +=  speedIncrement;
        if (currentGameSpeed > maxSpeed) {
            currentGameSpeed = maxSpeed;
        }
        SetState(State.Running);
    }

    public void DecreaseGameSpeed() {
        currentGameSpeed -= speedIncrement;
        if (currentGameSpeed < minSpeed) {
            currentGameSpeed = minSpeed;
        }
        SetState(State.Running);
    }

    private void TogglePause() {
        SetState(CurrentState == State.Paused ? State.Running : State.Paused);
    }

    private void LimitFramerate() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LevelSelect() {
        SceneManager.LoadScene("LevelSelect");
    }
}
