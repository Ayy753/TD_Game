using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager: IInitializable, IDisposable {
    IGUIManager guiController;
    
    public int Lives { get; private set; }

    public State CurrentState { get; private set; }

    private const int startingLives = 25;

    public GameManager(IGUIManager guiController) {
        this.guiController = guiController;
    }

    public enum State {
        Running,
        Paused,
        Ended
    }

    public void Initialize() {
        Debug.Log("GameManager initializing");
        Lives = startingLives;
        guiController.UpdateLivesLabel(Lives);
        Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
        InputHandler.OnCommandEntered += HandlekeyboardInput;
        SetState(State.Running);
    }

    public void Dispose() {
        Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
        InputHandler.OnCommandEntered += HandlekeyboardInput;
    }

    private void HandlekeyboardInput(InputHandler.Command command) {
        switch (command) {
            case InputHandler.Command.TogglePause:
                TogglePause();
                break;
            case InputHandler.Command.SlowGameSpeed:
                Debug.LogWarning("SlowGameSpeed not implemented yet");
                break;
            case InputHandler.Command.IncreaseGameSpeed:
                Debug.LogWarning("IncreaseGameSpeed not implemented yet");
                break;
        }
    }

    private void HandleEnemyReachedGate(Enemy enemy) {
        Lives -= 1;
        guiController.UpdateLivesLabel(Lives);
        if (Lives <= 0) {
            guiController.ShowGameOverScreen();
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
                Time.timeScale = 1;
                break;
            case State.Paused:
                Time.timeScale = 0;
                break;
            case State.Ended:
                Time.timeScale = 0;
                break;
            default:
                break;
        }

        CurrentState = state;
    }

    private void TogglePause() {
        SetState(CurrentState == State.Paused ? State.Running : State.Paused);
    }

    /// <summary>
    /// used by wavemanager when there are no more waves and the last enemy died or reached gate
    /// </summary>
    public void NoEnemiesLeft() {
        guiController.ShowGameWonScreen();
        SetState(State.Ended);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
