using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager: IInitializable {
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
    }
}
