namespace DefaultNamespace {

    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Zenject;

    /// <summary>
    /// Saves and loads user's level progress
    /// </summary>
    public class LevelManager : IInitializable, IDisposable {
        List<LevelData> levelData;
        private string filePath = "LevelData/PlayerProgress";
        public State CurrentState { get; private set; }
        public string CurrentLevelName { get; private set; }

        public enum State {
            InGame,
            LevelSelection,
            TestScene
        }

        public void Initialize() {
            Debug.Log("starting levelmanager");
            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == "LevelSelect") {
                LoadLevelData();
                CurrentState = State.LevelSelection;
            }
            else if (sceneName == "Testing Environment") {
                CurrentState = State.TestScene;
            }
            else {
                CurrentState = State.InGame;
                LoadLevelData();
                CurrentLevelName = sceneName;
            }
        }

        public void Dispose() {
            if (CurrentState == State.InGame) {
                SaveLevelData();
            }
        }

        /// <summary>
        /// Load player's progress data
        /// </summary>
        private void LoadLevelData() {
            string jsonText = ((TextAsset)Resources.Load(filePath, typeof(TextAsset))).text;
            levelData = JsonConvert.DeserializeObject<Root>(jsonText).LevelData;
        }

        /// <summary>
        /// Save player's progres data
        /// </summary>
        private void SaveLevelData() {
            Root root = new Root();
            root.LevelData = levelData;
            string jsonText = JsonConvert.SerializeObject(root);
            File.WriteAllText(Application.dataPath + "/Resources/" + filePath + ".txt", jsonText);
        }

        /// <summary>
        /// Sets the level's score
        /// </summary>
        /// <param name="level">Level index</param>
        /// <param name="score">Score between 0 and 5</param>
        public void SetLevelData(int level, int score) {
            if (level < levelData.Count && level > 0) {
                levelData[level].Score = score;
            }
            else {
                throw new ArgumentOutOfRangeException();
            }
        }

        public int GetLevelData(int level) {
            return levelData[level].Score;
        }

        public List<LevelData> GetLevelData() {
            return levelData;
        }

        //  Json objects
        public class LevelData {
            public string LevelName { get; set; }
            public int Score { get; set; }
        }

        public class Root {
            public List<LevelData> LevelData { get; set; }
        }
    }
}
