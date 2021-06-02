using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaveManager : IInitializable {

    [Inject] private ObjectPool objectPool;
    [Inject] private AsyncProcessor asyncProcessor;
    private const string FilePath = "LevelData/WaveData/demo_waves";
    private Root LevelData;

    public int NumberOfWaves { get; private set; }
    private int currentWave = 0;

    private const int timeBetweenWaves = 30;
    private const int timeBeforeFirstWave = 3;
    private Coroutine nextWaveCountDown;

    public void Initialize() {
        Debug.Log("Initializing WaveManager");
        LoadWaveData();
        nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
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
            //Debug.Log("Time until next wave: " + secondsUntilNextWave);
            yield return new WaitForSeconds(1f);
            secondsUntilNextWave--;
        }

        asyncProcessor.StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave() {
        int thisWaveNum = currentWave;
        currentWave++;

        asyncProcessor.StopCoroutine(nextWaveCountDown);

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
                Enemy enemy = objectPool.Create(groupType);
                enemy.Spawn();
            }

            yield return new WaitForSeconds(LevelData.waves[thisWaveNum].TimebetweenGroups);
        }

        Debug.Log(string.Format("currentwave-1: {0}, thisWaveNum: {1}, numwaves-1: {2}", (currentWave-1), thisWaveNum, ( NumberOfWaves -1)));

        if (currentWave-1 == thisWaveNum && currentWave < NumberOfWaves - 1) {
            nextWaveCountDown = asyncProcessor.StartCoroutine(NextWaveCountDown());
        }
    }
}
