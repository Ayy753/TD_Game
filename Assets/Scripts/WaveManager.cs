using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class WaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = "Assets/LevelData/demo_waves.json";
        string jsonText = System.IO.File.ReadAllText(path);
        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonText);

        foreach (Wave wave in myDeserializedClass.waves)
        {
            foreach (Group group in wave.Groups)
            {
                Debug.Log(string.Format("Enemy Type: {0}, Num enemies: {1}, Time between spawns: {2}",
                    group.EnemyType, group.NumEnemies, group.TimebetweenSpawns)); 
            }
        }
    }

    /// <summary>
    /// A group of enemies
    /// </summary>
    public class Group
    {
        public string EnemyType { get; set; }
        public int NumEnemies { get; set; }
        public double TimebetweenSpawns { get; set; }
    }

    /// <summary>
    /// A wave containing groups of enemies
    /// </summary>
    public class Wave
    {
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


}
