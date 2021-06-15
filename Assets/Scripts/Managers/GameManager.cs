using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager: IInitializable, IDisposable {
    IGUIManager guiController;
    
    public int Lives { get; private set; }

    public State CurrentState { get; private set; }

    private const int startingLives = 25;

    private const float minSpeed = 1f;
    private const float maxSpeed = 3f;
    private const float speedIncrement = 1f;
    private float currentGameSpeed;

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
        currentGameSpeed = minSpeed;
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
            case InputHandler.Command.DecreaseGameSpeed:
                DecreaseGameSpeed();
                break;
            case InputHandler.Command.IncreaseGameSpeed:
                IncreaseGameSpeed();
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
                Time.timeScale = currentGameSpeed;
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

    private void IncreaseGameSpeed() {
        currentGameSpeed +=  speedIncrement;
        if (currentGameSpeed > maxSpeed) {
            currentGameSpeed = maxSpeed;
        }
        SetState(State.Running);
    }

    private void DecreaseGameSpeed() {
        currentGameSpeed -= speedIncrement;
        if (currentGameSpeed < minSpeed) {
            currentGameSpeed = minSpeed;
        }
        SetState(State.Running);
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
