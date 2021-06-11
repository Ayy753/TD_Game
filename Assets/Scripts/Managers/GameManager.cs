using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager: IInitializable, IDisposable {
    IGUIManager guiController;
    
    public int Lives { get; private set; }
    public bool GameEnded { get; private set; }
    private const int startingLives = 25;

    public GameManager(IGUIManager guiController) {
        this.guiController = guiController;
    }

    public void Initialize() {
        Debug.Log("GameManager initializing");
        Lives = startingLives;
        guiController.UpdateLivesLabel(Lives);
        Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
        GameEnded = false;
        ResumeGame();
    }

    public void Dispose() {
        Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
        Debug.Log("Gameamanger Destroyed");
    }

    private void HandleEnemyReachedGate(Enemy enemy) {
        Lives -= 1;
        guiController.UpdateLivesLabel(Lives);
        if (Lives <= 0) {
            guiController.ShowGameOverScreen();
            PauseGame();
        }
    }

    /// <summary>
    /// used by wavemanager when there are no more waves and the last enemy died or reached gate
    /// </summary>
    public void NoEnemiesLeft() {
        GameEnded = true;
        guiController.ShowGameWonScreen();
        PauseGame();
    }

    private void PauseGame() {
        Time.timeScale = 0;
    }

    private void ResumeGame() {
        Time.timeScale = 1;
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
