namespace DefaultNamespace.GUI {
    
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Zenject;

    public class LevelSelectionPanel : IInitializable {
        private readonly LevelManager levelManager;

        public LevelSelectionPanel(LevelManager levelManager) {
            this.levelManager = levelManager;
        }

        public void Initialize() {
            Debug.Log("starting levelselectionpanel");
            PopulateLevelButtons();
        }

        private void PopulateLevelButtons() {
            Transform pnlGrid = GameObject.Find("pnlGrid").GetComponent<Transform>();
            GameObject levelButtonPrefab = Resources.Load<GameObject>("prefabs/btnLevel");
            List<LevelManager.LevelData> levelData = levelManager.GetLevelData();

            foreach (var level in levelData) {
                GameObject button = GameObject.Instantiate(levelButtonPrefab);
                TMP_Text txtLevelName = button.transform.Find("pnlLevelName/txtLevelName").GetComponent<TMP_Text>();
                txtLevelName.text = level.LevelName.ToString();
                button.transform.SetParent(pnlGrid);
                button.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level.LevelName); });
                button.transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Button's onclick will call this function
        /// </summary>
        public void LoadLevel(string levelName) {
            SceneManager.LoadScene(levelName);
        }
    }
}
