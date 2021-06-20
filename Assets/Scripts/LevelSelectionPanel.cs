using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionPanel : MonoBehaviour {
    private List<GameObject> levelSelectButtons = new List<GameObject>();
    private GameObject levelButtonPrefab;

    private void Awake() {
        levelButtonPrefab = Resources.Load<GameObject>("prefabs/btnLevelSelect");
        PopulateLevelButtons();
    }

    private void PopulateLevelButtons() {
        //GameObject.Instantiate(levelButtonPrefab);
    }

    /// <summary>
    /// Button's onclick will call this function
    /// </summary>
    public void LoadLevel() {
        int tempLevelNum = 0;
        SceneManager.LoadScene("level" + tempLevelNum);
    }
}
