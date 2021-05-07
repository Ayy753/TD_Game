using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.IO;

public class WaveManager : MonoBehaviour
{
    GUIController guiController;
    ObjectPool objectPool;

    //  Todo: in future search leveldata folder for a json file whose name matches the scene name
    private string FilePath = "Assets/LevelData/demo_waves.json";
    private Root LevelData;
    public int NumberOfWaves { get; private set; }
    public int CurrentWave { get; private set; }

    private int timeBetweenWaves = 5;

    [SerializeField]
    private GameObject[] enemyPrefabs;
    GameObject entrance;

    // Start is called before the first frame update
    void Start()
    {
        guiController = GameManager.Instance.GUIController;
        objectPool = GameManager.Instance.ObjectPool;
        entrance = GameObject.Find("Entrance");


        string jsonText = System.IO.File.ReadAllText(FilePath);
        LevelData = JsonConvert.DeserializeObject<Root>(jsonText);
        NumberOfWaves = LevelData.waves.Count;
        CurrentWave = 0;
    }

    private void OnEnable()
    {
        PathFinder.OnInitialPathCalculated += HandleInitialPathCalculated;
    }

    private void OnDisable()
    {
        PathFinder.OnInitialPathCalculated -= HandleInitialPathCalculated;
    }

    /// <summary>
    /// A group of enemies
    /// </summary>
    public class Group
    {
        public string EnemyType { get; set; }
        public int NumEnemies { get; set; }
        public float TimebetweenSpawns { get; set; }

        public override string ToString()
        {
            return string.Format("{0} X{1}",
                    EnemyType, NumEnemies);
        }
    }

    /// <summary>
    /// A wave containing groups of enemies
    /// </summary>
    public class Wave
    {
        public int WaveNum { get; set; }
        public List<Group> Groups { get; set; }
        public int TimebetweenGroups { get; set; }

        public override string ToString()
        {
            string result = string.Empty;
            foreach (Group group in Groups)
            {
                result += group.ToString() + "\n";
            }
            return result;
        }
    }

    /// <summary>
    /// The root of the json file containing the array of waves for this level
    /// </summary>
    public class Root
    {
        public List<Wave> waves { get; set; }
    }

    /// <summary>
    /// For debugging
    /// </summary>
    private void PrintWaveData()
    {
        foreach (Wave wave in LevelData.waves)
        {
            foreach (Group group in wave.Groups)
            {
                Debug.Log(string.Format("Enemy Type: {0}, Num enemies: {1}, Time between spawns: {2}",
                    group.EnemyType, group.NumEnemies, group.TimebetweenSpawns));
            }
        }
    }

    /// <summary>
    /// Starts spawning after the path is first calculated at the beginning of level
    /// </summary>
    private void HandleInitialPathCalculated()
    {
        StartCoroutine(NextWaveCountdown());
        guiController.UpdateWaveInformation(string.Empty, "Next wave's groups:\n" + LevelData.waves[0].ToString());
    }

    /// <summary>
    /// Start spawning enemies
    /// </summary>
    public void StartSpawning()
    {
        string strCurrent = string.Empty;
        string strNext = string.Empty;

        if (CurrentWave < NumberOfWaves)
        {
            strCurrent = "Current wave's groups:\n" + LevelData.waves[CurrentWave].ToString();

            if (CurrentWave +1 < NumberOfWaves)
            {
                strNext = "Next wave's groups:\n" + LevelData.waves[CurrentWave+1].ToString();
            }

            guiController.UpdateWaveInformation(strCurrent, strNext);
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            guiController.SpawnFloatingText(pos, "Starting wave " + (CurrentWave+1), Color.white, 3);
            StartCoroutine(StartNextWave());
        }
        else
        {
            Debug.Log("No more waves");
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    private IEnumerator StartNextWave()
    {
        if (CurrentWave < NumberOfWaves)
        {
            Wave wave = LevelData.waves[CurrentWave];
            StopCoroutine(NextWaveCountdown());
            Debug.Log("Starting wave " + (CurrentWave));
            CurrentWave++;

            //  Loop through each group of enemies
            foreach (WaveManager.Group group in wave.Groups)
            {
                //  Loop through each enemy in the current group
                for (int i = 0; i < group.NumEnemies; i++)
                {
                    //  Loop through list of prefabs to find one that matches the enemy type
                    foreach (GameObject enemyPrefab in enemyPrefabs)
                    {
                        if (enemyPrefab.name == group.EnemyType)
                        {
                            SpawnEnemy(entrance.transform.position, enemyPrefab);
                        }
                    }
                    //  Wait until next enemy should be spawned
                    yield return new WaitForSeconds(group.TimebetweenSpawns);
                }
                //  Wait until next group should start spawning
                yield return new WaitForSeconds(wave.TimebetweenGroups);
            }

            Debug.Log(string.Format("Wave {0} finished spawning", wave.WaveNum));
            if (CurrentWave - 1 == wave.WaveNum)
            {
                if (StillMoreWaves() == true)
                {
                    Debug.Log(string.Format("Wave {0} was the most recent wave launched.", wave.WaveNum));
                    StartCoroutine(NextWaveCountdown());
                }
            }
        }
    }



    /// <summary>
    /// Spawns an enemy at a specified position using the object pool
    /// </summary>
    /// <param name="position"></param>
    private void SpawnEnemy(Vector3 position, GameObject desiredEnemyPrefab)
    {
        EnemyData desiredEnemyData = desiredEnemyPrefab.GetComponentInChildren<Enemy>().EnemyData;
        Enemy newEnemy = objectPool.CreateEnemy(desiredEnemyData);
        newEnemy.Spawn(position);
    }

    /// <summary>
    /// Are there more waves?
    /// </summary>
    /// <returns></returns>
    public bool StillMoreWaves()
    {
        if (NumberOfWaves - CurrentWave > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Counts down and launches next wave when its ready
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextWaveCountdown()
    {
        int secondsUntilNextWave = timeBetweenWaves;
        while (secondsUntilNextWave > 0)
        {
            secondsUntilNextWave--;
            guiController.UpdateWaveCounter("Next wave in: \n" + secondsUntilNextWave);
            yield return new WaitForSeconds(1);
        }
        guiController.UpdateWaveCounter(string.Empty);
        StartSpawning();
    }
}
