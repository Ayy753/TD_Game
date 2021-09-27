namespace DefaultNamespace {

    using System;
    using System.Collections.Generic;

    public enum WaveState {
        StartOfLevel,
        WaveInProgress,
        WaitingBetweenWaves,
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

    public class WaveCountdownEventArgs : EventArgs {
        public int CountDown { get; private set; }

        public WaveCountdownEventArgs(int countDown) {
            CountDown = countDown;
        }
    }

    public class WaveNumberEventArgs : EventArgs {
        public int CurrentWaveNum { get; private set; }
        public int MaxWaves { get; private set; }

        public WaveNumberEventArgs(int currentWave, int maxWaves) {
            CurrentWaveNum = currentWave;
            MaxWaves = maxWaves;
        }
    }

    public class EnemiesRemainingEventArgs : EventArgs {
        public int EnemiesRemaining { get; private set; }

        public EnemiesRemainingEventArgs(int enemiesRemaining) {
            EnemiesRemaining = enemiesRemaining;
        }
    }

    public interface IWaveManager {
        public delegate void WaveStateChangedEventHandler(object sender, WaveStateChangedEventArgs args);
        public event WaveStateChangedEventHandler OnWaveStateChanged;

        public delegate void PlayerEndedWaveEventHandler(object sender, PlayerEndedWaveEventArgs args);
        public event PlayerEndedWaveEventHandler OnPlayerEndedWave;

        public delegate void WaveCountDownEventHandler(object sender, WaveCountdownEventArgs args);
        public event WaveCountDownEventHandler OnCountDownChanged;

        public delegate void WaveNumberEventHandler(object sender, WaveNumberEventArgs args);
        public event WaveNumberEventHandler OnWaveNumberChanged;

        public delegate void EnemiesRemainingEventHandler(object sender, EnemiesRemainingEventArgs args);
        public event EnemiesRemainingEventHandler OnEnemiesRemainingChanged;

        public int NumberOfWaves { get; }
        public void StartNextWave();
        public void EndActiveWaves();
        public float GetHealthModifierForNextWave();

        public Dictionary<EnemyData.EnemyType, int> GetCurrentWaveInfo();
        float GetValueModifierForNextWave();
    }
}
