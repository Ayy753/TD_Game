using System;
using UnityEngine;
using Zenject;

public class GameManager: IInitializable, IDisposable {
    IGUIManager guiController;
    
    public int Lives { get; private set; }
    public bool GameEnded { get; internal set; }
    private const int startingLives = 25;

    public GameManager(IGUIManager guiController) {
        this.guiController = guiController;
    }

    public void Initialize() {
        Lives = startingLives;
        guiController.UpdateLivesLabel(Lives);
        Enemy.OnEnemyReachedGate += HandleEnemyReachedGate;
    }

    public void Dispose() {
        Enemy.OnEnemyReachedGate -= HandleEnemyReachedGate;
    }

    private void HandleEnemyReachedGate(Enemy enemy) {
        Lives -= 1;
        guiController.UpdateLivesLabel(Lives);
        if (Lives <= 0) {
            Debug.Log("game over");
            //  TODO: Gameover logic
        }
    }
}
