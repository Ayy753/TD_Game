namespace DefaultNamespace.IO.WaveData {

    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// The root of the json file containing the array of waves for this level
    /// </summary>
    public class Root {
        public List<Wave> Waves { get; set; }
    }

    /// <summary>
    /// A wave containing groups of enemies
    /// </summary>
    public class Wave {
        public List<Group> Groups { get; set; }
        public int TimebetweenGroups { get; set; }

        public override string ToString() {
            StringBuilder result = new StringBuilder();

            foreach (Group group in Groups) {
                result.Append(group.ToString() + "\n");
            }
            return result.ToString();
        }
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

    public class WaveParser{
        private readonly LevelManager levelManager;
        private const string FolderPath = "LevelData/WaveData/";

        public WaveParser(LevelManager levelManager) {
            this.levelManager = levelManager;
        }

        public Root LoadWaveData() {
            string filePath = FolderPath + levelManager.CurrentLevelName;
            string jsonText = ((TextAsset)Resources.Load(filePath, typeof(TextAsset))).text;
            return JsonConvert.DeserializeObject<Root>(jsonText);
        }
    }
}