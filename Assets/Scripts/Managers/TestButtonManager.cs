namespace DefaultNamespace.GUI {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using TMPro;

    public class TestButtonManager : ButtonManager, IInitializable {
        private readonly EnemySpawner enemySpawner;
        private readonly IWallet wallet;
        private TMP_Dropdown spawnDropdown;
        Dictionary<int, EnemyData> enemyIdToEnemyData;

        public TestButtonManager(BuildManager buildManager, IWaveManager waveManager, GameManager gameManager, EnemySpawner enemySpawner, IWallet wallet, IGUIManager guiController) : base(buildManager, waveManager, gameManager, guiController) {
            Debug.Log("test build manager");
            this.enemySpawner = enemySpawner;
            this.wallet = wallet;
        }

        public new void Initialize() {
            base.Initialize();

            spawnDropdown = GameObject.Find("dropdownSpawn").GetComponent<TMP_Dropdown>();
            enemyIdToEnemyData = enemySpawner.EnemyIdToEnemyData();
            BindButtonsInScene();
        }

        public new void BindButtonsInScene() {
            foreach (EnemyData enemy in enemyIdToEnemyData.Values) {
                spawnDropdown.options.Add(new TMP_Dropdown.OptionData(enemy.Name));
            }

            Button btnSpawn = GameObject.Find("btnSpawn").GetComponent<Button>();
            Button btnAddGold = GameObject.Find("btnAddGold").GetComponent<Button>();
            Button btnAddLife = GameObject.Find("btnAddLife").GetComponent<Button>();
            Button btnRemoveLife = GameObject.Find("btnRemoveLife").GetComponent<Button>();

            btnSpawn.onClick.AddListener(delegate { TrySpawningSelectedEnemy(); });
            btnAddGold.onClick.AddListener(delegate { wallet.GainMoney(100); });
            btnAddLife.onClick.AddListener(delegate { gameManager.GainLife(); });
            btnRemoveLife.onClick.AddListener(delegate { gameManager.LoseLife(); });
        }

        private void TrySpawningSelectedEnemy() {
            try {
                int id = CurrentlySelectedDropdownOption();
                enemySpawner.SpawnEnemy(id);
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        private int CurrentlySelectedDropdownOption() {
            foreach (EnemyData enemyData in enemyIdToEnemyData.Values) {
                if (spawnDropdown.options[spawnDropdown.value].text == enemyData.Name) {
                    return enemyData.EnemyId;
                }
            }

            throw new Exception("Could not find ID of selected dropdown option");
        }
    }
}
