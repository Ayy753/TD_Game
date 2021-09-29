namespace DefaultNamespace.GUI {

    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class WaveReportPanel : IInitializable, IDisposable {
        private IWaveManager waveManager;
        private GameObject pnlWaveReport;
        private GameObject scrollViewContent;
        private GameObject reportRowPrefab;

        private Dictionary<int, EnemyData> enemyTypeToEnemyData;

        public WaveReportPanel(IWaveManager waveManager) {
            this.waveManager = waveManager;
        }

        public void Initialize() {
            waveManager.OnWaveStateChanged += WaveManager_OnWaveStateChanged;

            Debug.Log("initializing wave report panel");
            pnlWaveReport = GameObject.Find("pnlWaveReport");
            scrollViewContent = GameObject.Find("pnlWaveReport/Scroll View/Viewport/Content");
            reportRowPrefab = Resources.Load<GameObject>("Prefabs/pnlWaveReportRow");

            enemyTypeToEnemyData = new Dictionary<int, EnemyData>();
            EnemyData[] enemyDatas = Resources.LoadAll<EnemyData>("ScriptableObjects/EnemyData");

            foreach (EnemyData enemyData in enemyDatas) {
                enemyTypeToEnemyData.Add(enemyData.EnemyId, enemyData);
            }

            //  Generate initial wave report
            GenerateScoutReport();
        }
        
        public void Dispose() {
            waveManager.OnWaveStateChanged -= WaveManager_OnWaveStateChanged;
        }

        private void WaveManager_OnWaveStateChanged(object sender, WaveStateChangedEventArgs args) {
            GenerateScoutReport();
        }

        private void GenerateScoutReport() {
            Dictionary<int, int> enemyIdToAmount = waveManager.GetCurrentWaveInfo();
            RemoveAllReportRows();

            foreach (int enemyId in enemyIdToAmount.Keys) {
                int enemyAmount = enemyIdToAmount[enemyId];
                CreateReportRow(enemyId, enemyAmount);
            }
        }

        private void RemoveAllReportRows() {
            for (int i = 0; i < scrollViewContent.transform.childCount; i++) {
                GameObject.Destroy(scrollViewContent.transform.GetChild(i).gameObject);
            }
        }

        private void CreateReportRow(int enemyId, int enemyAmount) {
            GameObject row = GameObject.Instantiate(reportRowPrefab, scrollViewContent.transform);

            Image icon = row.transform.Find("imgIcon").GetComponent<Image>();
            TMP_Text txtType = row.transform.Find("pnlInfo/pnlTypeAndAmount/txtType").GetComponent<TMP_Text>();
            TMP_Text txtAmount = row.transform.Find("pnlInfo/pnlTypeAndAmount/txtAmount").GetComponent<TMP_Text>();

            EnemyData enemyData = enemyTypeToEnemyData[enemyId];
            txtType.text = enemyData.Name;
            txtAmount.text = enemyAmount.ToString();
            icon.sprite = enemyData.Icon;

            EnemyIcon enemyIcon = icon.GetComponent<EnemyIcon>();
            enemyIcon.EnemyData = enemyData;
            enemyIcon.SetHealthModifier(waveManager.GetHealthModifierForNextWave());
            enemyIcon.SetValueModifier(waveManager.GetValueModifierForNextWave());
        }

        public void ToggleWaveReport() {
            if (pnlWaveReport.activeInHierarchy == true) {
                pnlWaveReport.SetActive(false);
            }
            else {
                pnlWaveReport.SetActive(true);
            }
        }

        public bool IsWaveReportOpen() {
            return pnlWaveReport.activeInHierarchy;
        }
    }
}
