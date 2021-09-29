namespace DefaultNamespace {

    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class TestWaveManager : IWaveManager, IInitializable {
        private readonly IMessageSystem messageSystem;

        public event IWaveManager.WaveStateChangedEventHandler OnWaveStateChanged;
        public event IWaveManager.PlayerEndedWaveEventHandler OnPlayerEndedWave;
        public event IWaveManager.WaveCountDownEventHandler OnCountDownChanged;
        public event IWaveManager.WaveNumberEventHandler OnWaveNumberChanged;
        public event IWaveManager.EnemiesRemainingEventHandler OnEnemiesRemainingChanged;

        public int NumberOfWaves { get; private set; }

        public TestWaveManager(IMessageSystem messageSystem) {
            this.messageSystem = messageSystem;
        }

        public void Initialize() {
            OnWaveNumberChanged?.Invoke(null, new WaveNumberEventArgs(0, 0));
            OnCountDownChanged?.Invoke(null, new WaveCountdownEventArgs(0));
        }

        public void StartNextWave() {
            messageSystem.DisplayMessage("There are no waves in this test environment", Color.white);
        }

        public Dictionary<int, int> GetCurrentWaveInfo() {
            Debug.LogWarning("There are no waves in this test environment");
            return new Dictionary<int, int>();
        }

        public void EndActiveWaves() {
            messageSystem.DisplayMessage("There are no waves in this test environment", Color.white);
        }

        public float GetHealthModifierForNextWave() {
            throw new System.NotImplementedException();
        }

        public float GetValueModifierForNextWave() {
            throw new System.NotImplementedException();
        }
    }
}
