using System;
using System.Collections.Generic;

public interface IWaveManager {
    public delegate void WaveStateChangedEventHandler(object sender, WaveStateChangedEventArgs args);
    public event WaveStateChangedEventHandler OnWaveStateChanged;

    public delegate void PlayerEndedWaveEventHandler(object sender, PlayerEndedWaveEventArgs args);
    public event PlayerEndedWaveEventHandler OnPlayerEndedWave;

    public enum State {
        Waiting,
        WaveInProgress,
        LastWaveFinished
    }

    public int NumberOfWaves { get; }
    public void StartNextWave();
    public void EndActiveWaves();

    public Dictionary<EnemyData.EnemyType, int> GetCurrentWaveInfo();
}

public class WaveStateChangedEventArgs : EventArgs {
    public IWaveManager.State newState;

    public WaveStateChangedEventArgs(IWaveManager.State newState) {
        this.newState = newState;
    }
}

public class PlayerEndedWaveEventArgs : EventArgs {
    public int NumEnemiesRemaining { get; set; }
}