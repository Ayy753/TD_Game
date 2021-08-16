using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TestWaveManager : IWaveManager, IInitializable {
    private IMessageSystem messageSystem;
    private IGUIManager guiManager;

    public event IWaveManager.WaveStateChangedEventHandler OnWaveStateChanged;
    public event IWaveManager.PlayerEndedWaveEventHandler OnPlayerEndedWave;

    public int NumberOfWaves { get; private set; }

    public TestWaveManager(IMessageSystem messageSystem, IGUIManager guiManager) {
        this.messageSystem = messageSystem;
        this.guiManager = guiManager;
    }

    public void Initialize() {
        guiManager.UpdateWaveNumber(0, 0);
        guiManager.UpdateWaveCountdown(0);
    }

    public void StartNextWave() {
        messageSystem.DisplayMessage("There are no waves in this test environment", Color.white);
    }

    public Dictionary<EnemyData.EnemyType, int> GetCurrentWaveInfo() {
        Debug.LogWarning("There are no waves in this test environment");
        return null;
    }

    public void EndActiveWaves() {
        messageSystem.DisplayMessage("There are no waves in this test environment", Color.white);
    }
}
