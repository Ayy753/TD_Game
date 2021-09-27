namespace DefaultNamespace {

    using DefaultNamespace.GUI;
    using DefaultNamespace.IO.WaveData;
    using DefaultNamespace.StatusSystem;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class WaveManager : IWaveManager, IInitializable, IDisposable {
        private readonly EnemySpawner enemySpawner;
        private readonly AsyncProcessor asyncProcessor;
        private readonly IMessageSystem messageSystem;
        private readonly WaveParser waveParser;

        private List<Enemy> activeEnemies;

        private Root LevelData;

        public int NumberOfWaves { get; private set; }
        public float HealthModifier { get; private set; }
        private int mostRecentWaveNum = 0;
        private int numUnspawnedEnemiesSoFar;

        private const int timeBetweenWaves = 30;
        private Coroutine nextWaveCountDown;
        private readonly List<Coroutine> waveCoroutines = new List<Coroutine>();

        private bool lastWaveFinishedSpawning;
        private bool currentWaveFinishedSpawning;
        private WaveState currentState;
        private const float VALUE_SCALE_FACTOR = 50f;
        private const float HEALTH_SCALE_FACTOR = 25f;

        public event IWaveManager.WaveStateChangedEventHandler OnWaveStateChanged;
        public event IWaveManager.PlayerEndedWaveEventHandler OnPlayerEndedWave;
        public event IWaveManager.WaveCountDownEventHandler OnCountDownChanged;
        public event IWaveManager.WaveNumberEventHandler OnWaveNumberChanged;
        public event IWaveManager.EnemiesRemainingEventHandler OnEnemiesRemainingChanged;

        public WaveManager(EnemySpawner enemySpawner, AsyncProcessor asyncProcessor, IMessageSystem messageSystem, WaveParser waveParser) {
            this.enemySpawner = enemySpawner;
            this.asyncProcessor = asyncProcessor;
            this.messageSystem = messageSystem;
            this.waveParser = waveParser;
        }

        public void Initialize() {
            Debug.Log("Initializing WaveManager");

            Enemy.OnEnemyDied += HandleEnemyDeactivated;
            Enemy.OnEnemyReachedGate += HandleEnemyDeactivated;

            LevelData = waveParser.LoadWaveData();
            NumberOfWaves = LevelData.Waves.Count;

            activeEnemies = new List<Enemy>();
            lastWaveFinishedSpawning = false;
            currentWaveFinishedSpawning = false;

            mostRecentWaveNum = 0;

            OnCountDownChanged?.Invoke(null, new WaveCountdownEventArgs(0));
            OnWaveNumberChanged?.Invoke(null, new WaveNumberEventArgs(mostRecentWaveNum, NumberOfWaves));
            OnEnemiesRemainingChanged?.Invoke(null, new EnemiesRemainingEventArgs(0));
        }

        public void Dispose() {
            Enemy.OnEnemyDied -= HandleEnemyDeactivated;
            Enemy.OnEnemyReachedGate -= HandleEnemyDeactivated;
        }

        private void HandleEnemyDeactivated(Enemy enemy) {
            activeEnemies.Remove(enemy);
            UpdateEnemiesRemainingLabel();

            if (activeEnemies.Count == 0 && currentWaveFinishedSpawning) {
                if (!lastWaveFinishedSpawning) {
                    ChangeState(WaveState.WaitingBetweenWaves);
                }
                else {
                    ChangeState(WaveState.LastWaveFinished);
                }
            }
        }

        private void UpdateEnemiesRemainingLabel() {
            int enemiesRemaining = CalculateRemainingEnemiesSoFar();
            OnEnemiesRemainingChanged?.Invoke(null, new EnemiesRemainingEventArgs(enemiesRemaining));
        }

        private int CalculateRemainingEnemiesSoFar() {
            return numUnspawnedEnemiesSoFar + activeEnemies.Count;
        }

        private IEnumerator NextWaveCountDown() {
            int secondsUntilNextWave = timeBetweenWaves;

            while (secondsUntilNextWave > 0) {
                if (secondsUntilNextWave <= 5) {
                    messageSystem.DisplayMessage(string.Format("Next wave starts in {0} seconds", secondsUntilNextWave), Color.white, 1f);
                }
                yield return new WaitForSeconds(1f);
                secondsUntilNextWave--;
                OnCountDownChanged?.Invoke(null, new WaveCountdownEventArgs(secondsUntilNextWave));
            }

            StartNextWave();
        }

        private IEnumerator LaunchWave(int waveNum) {
            currentWaveFinishedSpawning = false;
            numUnspawnedEnemiesSoFar += GetTotalEnemyCountInWave(waveNum);
            mostRecentWaveNum++;
            ChangeState(WaveState.WaveInProgress);

            int numGroups = LevelData.Waves[waveNum].Groups.Count;

            for (int g = 0; g < numGroups; g++) {
                Group group = LevelData.Waves[waveNum].Groups[g];

                for (int i = 0; i < group.NumEnemies; i++) {
                    Enemy enemy = enemySpawner.SpawnEnemy(group.EnemyType);
                    ApplyWaveBuff(enemy.Status, waveNum);
                    ScaleEnemyValue(enemy);
                    activeEnemies.Add(enemy);
                    numUnspawnedEnemiesSoFar--;

                    //  Don't wait after last enemy in group
                    if (i < group.NumEnemies - 1) {
                        yield return new WaitForSeconds(group.TimebetweenSpawns);
                    }
                }

                //  Don't wait after last group in wave
                if (g < numGroups - 1) {
                    yield return new WaitForSeconds(LevelData.Waves[waveNum].TimebetweenGroups);
                }
            }

            if (waveNum == mostRecentWaveNum - 1) {
                currentWaveFinishedSpawning = true;
                if (waveNum == NumberOfWaves - 1) {
                    lastWaveFinishedSpawning = true;
                }
            }
        }

        private int GetTotalEnemyCountInWave(int waveNum) {
            int totalEnemies = 0;
            int numGroups = LevelData.Waves[waveNum].Groups.Count;

            for (int g = 0; g < numGroups; g++) {
                Group group = LevelData.Waves[waveNum].Groups[g];
                totalEnemies += group.NumEnemies;
            }
            Debug.Log($"total enemies this wave: {totalEnemies}");

            return totalEnemies;
        }

        public float GetHealthModifierForNextWave() {
            return CalculateWaveBuffPercentage(mostRecentWaveNum);
        }

        /// <summary>
        /// Calculates the percentage of the unit's base health that will be added based on wave number
        /// </summary>
        /// <param name="waveNum"></param>
        /// <returns></returns>
        private float CalculateWaveBuffPercentage(int waveNum) {
            return (float)(Math.Pow(waveNum, 2) / HEALTH_SCALE_FACTOR);
        }

        /// <summary>
        /// Applies a buff to unit's health based on wave number
        /// </summary>
        /// <param name="status"></param>
        /// <param name="waveNum"></param>
        private void ApplyWaveBuff(Status status, int waveNum) {
            float buffPercentage = CalculateWaveBuffPercentage(waveNum);
            float baseHealth = status.Health.Value;
            float buffAmount = Mathf.Round(buffPercentage * baseHealth);
            status.ModifyStat(StatType.Health, buffAmount);
        }

        private void ScaleEnemyValue(Enemy enemy) {
            int baseValue = enemy.EnemyData.BaseValue;
            enemy.ModifiedValue = (int)Mathf.Round( baseValue + baseValue * mostRecentWaveNum/VALUE_SCALE_FACTOR);
        }

        public float GetValueModifierForNextWave() {
            return mostRecentWaveNum/VALUE_SCALE_FACTOR;
        }

        private void ChangeState(WaveState state) {
            switch (state) {
                case WaveState.WaitingBetweenWaves:
                    if (nextWaveCountDown != null) {
                        asyncProcessor.StopCoroutine(nextWaveCountDown);
                    }

                    OnCountDownChanged?.Invoke(null, new WaveCountdownEventArgs(timeBetweenWaves));
                    nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
                    break;
                case WaveState.WaveInProgress:
                    if (nextWaveCountDown != null) {
                        asyncProcessor.StopCoroutine(nextWaveCountDown);
                    }

                    OnCountDownChanged?.Invoke(null, new WaveCountdownEventArgs(0));
                    OnWaveNumberChanged?.Invoke(null, new WaveNumberEventArgs(mostRecentWaveNum, NumberOfWaves));
                    UpdateEnemiesRemainingLabel();
                    break;
                case WaveState.LastWaveFinished:
                    break;
            }
                
            currentState = state;
            OnWaveStateChanged?.Invoke(this, new WaveStateChangedEventArgs(state));
        }

        public void StartNextWave() {
            if (mostRecentWaveNum < NumberOfWaves) {
                waveCoroutines.Add(asyncProcessor.StartCoroutine(LaunchWave(mostRecentWaveNum)));
                messageSystem.DisplayMessage("Starting wave " + mostRecentWaveNum, Color.white, 1f);
            }
        }

        public void EndActiveWaves() {
            if (currentState == WaveState.WaveInProgress) {
                StopActiveWaves();
                ChangeState(WaveState.WaitingBetweenWaves);
                ApplyWaveTerminationPenalties();
                ClearActiveEnemies();
                UpdateEnemiesRemainingLabel();
            }
        }

        private void StopActiveWaves() {
            for (int i = 0; i < waveCoroutines.Count; i++) {
                asyncProcessor.StopCoroutine(waveCoroutines[i]);
            }
            waveCoroutines.Clear();
        }

        private void ApplyWaveTerminationPenalties() {
            int totalRemainingEnemies = CalculateRemainingEnemiesSoFar();
            OnPlayerEndedWave?.Invoke(this, new PlayerEndedWaveEventArgs (totalRemainingEnemies));
            numUnspawnedEnemiesSoFar = 0;
        }

        private void ClearActiveEnemies() {
            for (int i = 0; i < activeEnemies.Count; i++) {
                activeEnemies[i].Despawn();
            }
            activeEnemies.Clear();
        }

        public Dictionary<EnemyData.EnemyType, int> GetCurrentWaveInfo() {
            Dictionary<EnemyData.EnemyType, int> enemyTypeToAmount = new Dictionary<EnemyData.EnemyType, int>();

            if (mostRecentWaveNum < LevelData.Waves.Count) {
                foreach (Group group in LevelData.Waves[mostRecentWaveNum].Groups) {
                    if (enemyTypeToAmount.ContainsKey(group.EnemyType)) {
                        enemyTypeToAmount[group.EnemyType] += group.NumEnemies;
                    }
                    else {
                        enemyTypeToAmount.Add(group.EnemyType, group.NumEnemies);
                    }
                }
            }
            return enemyTypeToAmount;
        }
    }
}
