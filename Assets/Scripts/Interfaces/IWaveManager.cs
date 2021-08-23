namespace DefaultNamespace {

    using System;
    using System.Collections.Generic;

    public enum WaveState {
        Waiting,
        WaveInProgress,
        LastWaveFinished
    }

    public class WaveStateChangedEventArgs : EventArgs {
        public WaveState NewState { get; private set; }

        public WaveStateChangedEventArgs(WaveState newState) {
            this.NewState = newState;
        }
    }

    public class PlayerEndedWaveEventArgs : EventArgs {
        public int NumEnemiesRemaining { get; private set; }

        public PlayerEndedWaveEventArgs(int numEnemiesRemaining) {
            NumEnemiesRemaining = numEnemiesRemaining;
        }
    }

    public interface IWaveManager {
        public delegate void WaveStateChangedEventHandler(object sender, WaveStateChangedEventArgs args);
        public event WaveStateChangedEventHandler OnWaveStateChanged;

        public delegate void PlayerEndedWaveEventHandler(object sender, PlayerEndedWaveEventArgs args);
        public event PlayerEndedWaveEventHandler OnPlayerEndedWave;

        public int NumberOfWaves { get; }
        public void StartNextWave();
        public void EndActiveWaves();

        public Dictionary<EnemyData.EnemyType, int> GetCurrentWaveInfo();
    }
}
