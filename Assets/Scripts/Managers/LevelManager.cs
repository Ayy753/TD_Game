using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zenject;

/// <summary>
/// Saves and loads user's level progress
/// </summary>
public class LevelManager : IInitializable, IDisposable{
    //private Dictionary<int, int> levelScores;
    List<LevelData> levelData;
    private string filePath = "LevelData/PlayerProgress";

    public void Initialize() {
        LoadLevelData();
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
    }

    /// <summary>
    /// Save player's progres data
    /// </summary>
    private void SaveLevelData() {
        Root root = new Root();
        root.LevelData = levelData;
        string jsonText = JsonConvert.SerializeObject(root);
        File.WriteAllText(Application.dataPath + "/Resources/" +  filePath + ".txt", jsonText);
        AssetDatabase.Refresh();
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

    //  Json objects
    public class LevelData {
        public int LevelNum { get; set; }
        public int Score { get; set; }
    }

    public class Root {
        public List<LevelData> LevelData { get; set; }
    }
}



public class LevelButton : MonoBehaviour {
    public int levelNum;
}
