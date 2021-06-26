using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LevelSelectionPanel : MonoBehaviour, IInitializable {
    [Inject] private LevelManager levelManager;

    private List<GameObject> levelSelectButtons = new List<GameObject>();
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
            TMP_Text txtLevel = button.transform.Find("txtLevel").GetComponent<TMP_Text>();
            txtLevel.text = level.LevelNum.ToString();
            button.transform.SetParent(pnlGrid);
        }
    }

    /// <summary>
    /// Button's onclick will call this function
    /// </summary>
    public void LoadLevel() {
        int tempLevelNum = 0;
        SceneManager.LoadScene("level" + tempLevelNum);
    }
}
