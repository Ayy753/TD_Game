using System;

public interface IWaveManager {
    public delegate void WaveStateChangedEventHandler(object sender, WaveStateChangedEventArgs args);
    public event WaveStateChangedEventHandler OnWaveStateChanged;

    public enum State {
        Waiting,
        WaveInProgress,
        LastWaveFinished
    }

    public int NumberOfWaves { get; }
    public void StartNextWave();
}

public class WaveStateChangedEventArgs : EventArgs {
    public IWaveManager.State newState;

    public WaveStateChangedEventArgs(IWaveManager.State newState) {
        this.newState = newState;
    }
}