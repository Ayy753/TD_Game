using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaveManager : IWaveManager, IInitializable, IDisposable {
    private EnemySpawner enemySpawner;
    private AsyncProcessor asyncProcessor;
    private IMessageSystem messageSystem;
    private IGUIManager guiController;
    private LevelManager levelManager;

    private const string FolderPath = "LevelData/WaveData/";
    private Root LevelData;

    private List<Enemy> activeEnemies;

    public int NumberOfWaves { get; private set; }
    private int mostRecentWaveNum = 0;

    private const int timeBetweenWaves = 30;
    private const int timeBeforeFirstWave = 60;
    private Coroutine nextWaveCountDown;

    private bool lastWaveFinishedSpawning;
    private bool currentWaveFinishedSpawning;
    private IWaveManager.State currentState;

    public event IWaveManager.WaveStateChangedEventHandler OnWaveStateChanged;

    public WaveManager(EnemySpawner enemySpawner, AsyncProcessor asyncProcessor, IMessageSystem messageSystem, IGUIManager guiController, LevelManager levelManager) {
        this.enemySpawner = enemySpawner;
        this.asyncProcessor = asyncProcessor;
        this.messageSystem = messageSystem;
        this.guiController = guiController;
        this.levelManager = levelManager;
    }


    public void Initialize() {
        Debug.Log("Initializing WaveManager");

        Enemy.OnEnemyDied += HandleEnemyDeactivated;
        Enemy.OnEnemyReachedGate += HandleEnemyDeactivated;


        LoadWaveData();

        nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
        activeEnemies = new List<Enemy>();
        ChangeState(IWaveManager.State.Waiting);
        lastWaveFinishedSpawning = false;
        currentWaveFinishedSpawning = false;

        mostRecentWaveNum = 0;

        messageSystem.DisplayMessage(string.Format("First wave starts in {0} seconds", timeBeforeFirstWave), Color.white, 1f);

        guiController.UpdateWaveCountdown(timeBeforeFirstWave);
        guiController.UpdateWaveNumber(mostRecentWaveNum, NumberOfWaves);
    }

    public void Dispose() {
        Enemy.OnEnemyDied -= HandleEnemyDeactivated;
        Enemy.OnEnemyReachedGate -= HandleEnemyDeactivated;
    }

    private void HandleEnemyDeactivated(Enemy enemy) {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0 && currentWaveFinishedSpawning == true) {
            if (lastWaveFinishedSpawning == false) {
                nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
                ChangeState(IWaveManager.State.Waiting);
            }
            else {
                ChangeState(IWaveManager.State.LastWaveFinished);
            }
        }
    }

    private void LoadWaveData() {
        string filePath = FolderPath + "Level_" + levelManager.CurrentLevel;
        string jsonText = ((TextAsset)Resources.Load(filePath, typeof(TextAsset))).text;
        LevelData = JsonConvert.DeserializeObject<Root>(jsonText);
        NumberOfWaves = LevelData.waves.Count;
    }


    /// <summary>
    /// A group of enemies
    /// </summary>
    public class Group {
        public EnemyData.EnemyType EnemyType { get; set; }
        public int NumEnemies { get; set; }
        public float TimebetweenSpawns { get; set; }

        public override string ToString() {
            return string.Format("{0} X{1}",
                    EnemyType, NumEnemies);
        }
    }

    /// <summary>
    /// A wave containing groups of enemies
    /// </summary>
    public class Wave {
        public List<Group> Groups { get; set; }
        public int TimebetweenGroups { get; set; }

        public override string ToString() {
            string result = string.Empty;
            foreach (Group group in Groups) {
                result += group.ToString() + "\n";
            }
            return result;
        }
    }

    /// <summary>
    /// The root of the json file containing the array of waves for this level
    /// </summary>
    public class Root {
        public List<Wave> waves { get; set; }
    }

    private IEnumerator NextWaveCountDown() {
        int secondsUntilNextWave;

        if (mostRecentWaveNum == 0) {
            secondsUntilNextWave = timeBeforeFirstWave;
        }
        else {
            secondsUntilNextWave = timeBetweenWaves;
        }

        while (secondsUntilNextWave > 0) {
            if (secondsUntilNextWave <= 5) {
                messageSystem.DisplayMessage(string.Format("Next wave starts in {0} seconds", secondsUntilNextWave), Color.white, 1f);
            }
            yield return new WaitForSeconds(1f);
            secondsUntilNextWave--;
            guiController.UpdateWaveCountdown(secondsUntilNextWave);
        }

        StartNextWave();
    }

    private IEnumerator LaunchWave(int waveNum) {
        currentWaveFinishedSpawning = false;
        mostRecentWaveNum++;
        ChangeState(IWaveManager.State.WaveInProgress);

        int numGroups = LevelData.waves[waveNum].Groups.Count;

        for (int g = 0; g < numGroups; g++) {
            Group group = LevelData.waves[waveNum].Groups[g];

            for (int i = 0; i < group.NumEnemies; i++) {
                Enemy enemy = enemySpawner.SpawnEnemy(group.EnemyType);
                ApplyWaveBuff(enemy.GetStatus(), waveNum);
                activeEnemies.Add(enemy);

                //  Don't wait after last enemy in group
                if (i < group.NumEnemies - 1) {
                    yield return new WaitForSeconds(group.TimebetweenSpawns);
                }
            }

            //  Don't wait after last group in wave
            if (g < numGroups - 1) {
                yield return new WaitForSeconds(LevelData.waves[waveNum].TimebetweenGroups);
            }
        }

        if (waveNum == mostRecentWaveNum - 1) {
            currentWaveFinishedSpawning = true;
            if (waveNum == NumberOfWaves - 1) {
                lastWaveFinishedSpawning = true;
            }
        }
    }

    /// <summary>
    /// Calculates the percentage of the unit's base health that will be added based on wave number
    /// </summary>
    /// <param name="waveNum"></param>
    /// <returns></returns>
    private float CalculateWaveBuffPercentage(int waveNum) {
        return (float)(Math.Pow(waveNum, 2) / 20);
    }

    /// <summary>
    /// Applies a buff to unit's health based on wave number
    /// </summary>
    /// <param name="status"></param>
    /// <param name="waveNum"></param>
    private void ApplyWaveBuff(Status status, int waveNum) {
        float buffPercentage = CalculateWaveBuffPercentage(waveNum);
        float baseHealth = status.Health.Value;
        float buffAmount = buffPercentage * baseHealth;
        status.ModifyStat(Status.StatType.Health, buffAmount);
    }

    private void ChangeState(IWaveManager.State state) {
        if (OnWaveStateChanged != null) {
            OnWaveStateChanged.Invoke(this, new WaveStateChangedEventArgs(state));
        }
        currentState = state;

        switch (state) {
            case IWaveManager.State.Waiting:
                guiController.ShowBuildMenu();
                break;
            case IWaveManager.State.WaveInProgress:
                guiController.HideBuildMenu();
                break;
            case IWaveManager.State.LastWaveFinished:
                break;
            default:
                break;
        }

        Debug.Log("wave manager switched state to " + state);
    }

    public void StartNextWave() {
        if (mostRecentWaveNum < NumberOfWaves) {
            asyncProcessor.StopCoroutine(nextWaveCountDown);
            guiController.UpdateWaveCountdown(0);
            asyncProcessor.StartCoroutine(LaunchWave(mostRecentWaveNum));
            messageSystem.DisplayMessage("Starting wave " + mostRecentWaveNum, Color.white, 1f);
            guiController.UpdateWaveNumber(mostRecentWaveNum, NumberOfWaves);
        }
    }

    public Dictionary<EnemyData.EnemyType, int> GetCurrentWaveInfo() {
        if (mostRecentWaveNum == NumberOfWaves) {
            return null;
        }
        
        Dictionary<EnemyData.EnemyType, int> enemyTypeToAmount = new Dictionary<EnemyData.EnemyType, int>();

        foreach (Group group in LevelData.waves[mostRecentWaveNum].Groups) {
            if (enemyTypeToAmount.ContainsKey(group.EnemyType)) {
                enemyTypeToAmount[group.EnemyType] += group.NumEnemies;
            }
            else {
                enemyTypeToAmount.Add(group.EnemyType, group.NumEnemies);
            }
        }

        return enemyTypeToAmount;
    }
}
