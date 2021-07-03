using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

/// <summary>
/// Saves and loads user's level progress
/// </summary>
public class LevelManager : IInitializable, IDisposable{
    List<LevelData> levelData;
    private string filePath = "LevelData/PlayerProgress";
    public int CurrentLevel { get; private set; }

    public void Initialize() {
        Debug.Log("starting levelmanager");
        LoadLevelData();

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName != "LevelSelect") {
            CurrentLevel = int.Parse(sceneName.Split('_')[1]);
        }
    }
     
    public void Dispose() {
        SaveLevelData();
    }

    /// <summary>
    /// Load player's progress data
    /// </summary>
    private void LoadLevelData() {
        string jsonText = ((TextAsset)Resources.Load(filePath, typeof(TextAsset))).text;
        levelData = JsonConvert.DeserializeObject<Root>(jsonText).LevelData;

        Debug.Log("leveldata length: " + levelData.Count);
    }

    /// <summary>
    /// Save player's progres data
    /// </summary>
    private void SaveLevelData() {
        Root root = new Root();
        root.LevelData = levelData;
        string jsonText = JsonConvert.SerializeObject(root);
        File.WriteAllText(Application.dataPath + "/Resources/" +  filePath + ".txt", jsonText);
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
        public int LevelNum { get; set; }
        public int Score { get; set; }
    }

    public class Root {
        public List<LevelData> LevelData { get; set; }
    }
}
