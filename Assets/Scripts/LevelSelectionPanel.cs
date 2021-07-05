using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LevelSelectionPanel : MonoBehaviour, IInitializable {
    [Inject] private LevelManager levelManager;

    private GameObject levelButtonPrefab;
    private Transform pnlGrid;

    public void Initialize() {
        Debug.Log("starting levelselectionpanel");
        pnlGrid = GameObject.Find("pnlGrid").GetComponent < Transform>() ;
        levelButtonPrefab = Resources.Load<GameObject>("prefabs/btnLevel");
        PopulateLevelButtons();
    }

    private void PopulateLevelButtons() {
        List<LevelManager.LevelData> levelData = levelManager.GetLevelData();
        foreach (var level in levelData) {
            GameObject button = GameObject.Instantiate(levelButtonPrefab);
            TMP_Text txtLevelNum = button.transform.Find("pnlLevelNum/txtLevelNum").GetComponent<TMP_Text>();
            TMP_Text txtLevelName = button.transform.Find("pnlLevelName/txtLevelName").GetComponent<TMP_Text>();
            txtLevelNum.text = level.LevelNum.ToString();
            txtLevelName.text = level.LevelName.ToString();
            button.transform.SetParent(pnlGrid);
            button.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level.LevelNum); });
        }
    }

    /// <summary>
    /// Button's onclick will call this function
    /// </summary>
    public void LoadLevel(int levelNum) {
        SceneManager.LoadScene("level_" + levelNum);
    }
}
