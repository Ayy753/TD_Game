using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaveManager : IInitializable, IDisposable {

    [Inject] private ObjectPool objectPool;
    [Inject] private AsyncProcessor asyncProcessor;
    [Inject] private GameManager gameManager;
    [Inject] private IMessageSystem messageSystem;
    [Inject] private IGUIManager guiController;

    private const string FilePath = "LevelData/WaveData/demo_waves";
    //private const string FilePath = "LevelData/WaveData/dummy_waves";
    private Root LevelData;

    private List<Enemy> activeEnemies;

    public int NumberOfWaves { get; private set; }
    private int currentWave = 0;

    private const int timeBetweenWaves = 30;
    private const int timeBeforeFirstWave = 60;
    private Coroutine nextWaveCountDown;
    private bool lastWaveFinishedSpawning;

    public void Initialize() {
        Debug.Log("Initializing WaveManager");

        Enemy.OnEnemyDied += HandleEnemyDeactivated;
        Enemy.OnEnemyReachedGate += HandleEnemyDeactivated;

        LoadWaveData();

        nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
        activeEnemies = new List<Enemy>();
        lastWaveFinishedSpawning = false;

        currentWave = 0;

        messageSystem.DisplayMessage(string.Format("First wave starts in {0} seconds", timeBeforeFirstWave), Color.white, 1f);

        guiController.UpdateWaveCountdown(timeBeforeFirstWave);
        guiController.UpdateWaveNumber(currentWave, NumberOfWaves);
    }

    public void Dispose() {
        Enemy.OnEnemyDied -= HandleEnemyDeactivated;
        Enemy.OnEnemyReachedGate -= HandleEnemyDeactivated;
    }

    private void HandleEnemyDeactivated(Enemy enemy) {
        activeEnemies.Remove(enemy);
        if (lastWaveFinishedSpawning && activeEnemies.Count == 0) {
            gameManager.NoEnemiesLeft();
        }
    }

    private void LoadWaveData() {
        string jsonText = ((TextAsset)Resources.Load(FilePath, typeof(TextAsset))).text;
        LevelData = JsonConvert.DeserializeObject<Root>(jsonText);
        NumberOfWaves = LevelData.waves.Count;
    }


    /// <summary>
    /// A group of enemies
    /// </summary>
    public class Group {
        public string EnemyType { get; set; }
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
        public int WaveNum { get; set; }
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

        if (currentWave == 0) {
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

    private IEnumerator LaunchWave() {
        int thisWaveNum = currentWave;
        currentWave++;
 
        foreach (Group group in LevelData.waves[thisWaveNum].Groups) {
            EnemyData.Type groupType;

            switch (group.EnemyType) {
                case "FastEnemy":
                    groupType = EnemyData.Type.Fast;
                    break;
                case "NormalEnemy":
                    groupType = EnemyData.Type.Normal;
                    break;
                case "StrongEnemy":
                    groupType = EnemyData.Type.Strong;
                    break;
                default:
                    throw new System.Exception(string.Format("Enemy type \"{0}\" does not match any of the prefabs in the resource folder", group.EnemyType));
            }

            for (int i = 0; i < group.NumEnemies; i++) {
                yield return new WaitForSeconds(group.TimebetweenSpawns);
                Enemy enemy = objectPool.CreateEnemy(groupType);
                enemy.Spawn();
                activeEnemies.Add(enemy);
            }

            yield return new WaitForSeconds(LevelData.waves[thisWaveNum].TimebetweenGroups);
        }

        //  If there are more waves, launch next wave
        if (currentWave < NumberOfWaves) {
            nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
        }
        //  Otherwise if the last wave is defeated
        else if(currentWave - 1 == thisWaveNum) {
            lastWaveFinishedSpawning = true;
        }
    }

    public void StartNextWave() {
        if (currentWave < NumberOfWaves) {
            asyncProcessor.StopCoroutine(nextWaveCountDown);
            guiController.UpdateWaveCountdown(0);
            messageSystem.DisplayMessage("Starting wave " + (currentWave + 1), Color.white, 1f);
            asyncProcessor.StartCoroutine(LaunchWave());
            guiController.UpdateWaveNumber(currentWave, NumberOfWaves);
        }
    }
}
