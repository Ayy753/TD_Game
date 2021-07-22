using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaveManager : IWaveManager, IInitializable, IDisposable {
    private EnemySpawner enemySpawner;
    private AsyncProcessor asyncProcessor;
    private GameManager gameManager;
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
    private State currentState;
    public delegate void StateChanged(State newState);
    public static event StateChanged OnStateChanged;

    public WaveManager(EnemySpawner enemySpawner, AsyncProcessor asyncProcessor, GameManager gameManager, IMessageSystem messageSystem, IGUIManager guiController, LevelManager levelManager) {
        this.enemySpawner = enemySpawner;
        this.asyncProcessor = asyncProcessor;
        this.gameManager = gameManager;
        this.messageSystem = messageSystem;
        this.guiController = guiController;
        this.levelManager = levelManager;
    }

    public enum State {
        Waiting,
        WaveInProgress,
        LastWaveFinished
    }

    public void Initialize() {
        Debug.Log("Initializing WaveManager");

        Enemy.OnEnemyDied += HandleEnemyDeactivated;
        Enemy.OnEnemyReachedGate += HandleEnemyDeactivated;


        LoadWaveData();

        nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
        activeEnemies = new List<Enemy>();
        currentState = State.Waiting;
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
                ChangeState(State.Waiting);
            }
            else {
                ChangeState(State.LastWaveFinished);
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
        ChangeState(State.WaveInProgress);

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

    private void ChangeState(State state) {
        if (OnStateChanged != null) {
            OnStateChanged.Invoke(state);
        }
        currentState = state;

        switch (state) {
            case State.Waiting:
                guiController.ShowBuildMenu();
                break;
            case State.WaveInProgress:
                guiController.HideBuildMenu();
                break;
            case State.LastWaveFinished:
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
            mostRecentWaveNum++;
            messageSystem.DisplayMessage("Starting wave " + mostRecentWaveNum, Color.white, 1f);
            guiController.UpdateWaveNumber(mostRecentWaveNum, NumberOfWaves);
        }
    }
}
