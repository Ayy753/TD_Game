using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class WaveManager : MonoBehaviour
{
    //  Todo: in future search leveldata folder for a json file whose name matches the scene name
    private string FilePath = "Assets/LevelData/demo_waves.json";
    private Root LevelData;
    public int NumberOfWaves { get; private set; }
    public int CurrentWave { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        string jsonText = System.IO.File.ReadAllText(FilePath);
        LevelData = JsonConvert.DeserializeObject<Root>(jsonText);
        NumberOfWaves = LevelData.waves.Count;
        CurrentWave = 0;
    }

    /// <summary>
    /// A group of enemies
    /// </summary>
    public class Group
    {
        public string EnemyType { get; set; }
        public int NumEnemies { get; set; }
        public float TimebetweenSpawns { get; set; }
    }

    /// <summary>
    /// A wave containing groups of enemies
    /// </summary>
    public class Wave
    {
        public int WaveNum { get; set; }
        public List<Group> Groups { get; set; }
        public int TimebetweenGroups { get; set; }
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
    /// Returns a wave object
    /// </summary>
    /// <param name="waveNum"></param>
    /// <returns></returns>
    public Wave GetNextWave()
    {
        if (CurrentWave < NumberOfWaves)
        {
            //  return the current wave and then increments the counter
            return LevelData.waves[CurrentWave++];
        }
        Debug.LogError("There aren't any more waves");
        return null;
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
}
