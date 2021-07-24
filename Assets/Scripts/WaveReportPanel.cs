using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WaveReportPanel : IInitializable, IDisposable{
    private IWaveManager waveManager;
    private GameObject pnlWaveReport;
    private GameObject scrollViewContent;
    private GameObject reportRowPrefab;

    private Dictionary<EnemyData.EnemyType, EnemyData> enemyTypeToEnemyData;

    public WaveReportPanel(IWaveManager waveManager) {
        this.waveManager = waveManager;
    }

    public void Initialize() {
        waveManager.OnWaveStateChanged += HandleWaveStateChanged;
     
        Debug.Log("initializing wave report panel");
        pnlWaveReport = GameObject.Find("pnlWaveReport");
        scrollViewContent = GameObject.Find("pnlWaveReport/Scroll View/Viewport/Content");
        reportRowPrefab = Resources.Load<GameObject>("Prefabs/pnlWaveReportRow");

        enemyTypeToEnemyData = new Dictionary<EnemyData.EnemyType, EnemyData>();
        EnemyData[] enemyDatas = Resources.LoadAll<EnemyData>("ScriptableObjects/EnemyData");

        foreach (EnemyData enemyData in enemyDatas) {
            enemyTypeToEnemyData.Add(enemyData.Type, enemyData);
        }

        //  Generate initial wave report
        GenerateScoutReport();
    }

    public void Dispose() {
        waveManager.OnWaveStateChanged -= HandleWaveStateChanged;
    }

    private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
        //  If a new wave is launched generate next wave report
        if (arg.newState == IWaveManager.State.WaveInProgress) {
            GenerateScoutReport();
        }
    }

    private void GenerateScoutReport() {
        Dictionary<EnemyData.EnemyType, int> enemyTypeToAmount = waveManager.GetCurrentWaveInfo();
        RemoveReportRows();

        if (enemyTypeToAmount != null) {
            foreach (EnemyData.EnemyType enemyType in enemyTypeToAmount.Keys) {
                int enemyAmount = enemyTypeToAmount[enemyType];
                CreateReportRow(enemyType, enemyAmount);
            }
        }
        else {
            Debug.Log("collection is null, must be the last wave");
        }
    }

    private void RemoveReportRows() {
        for (int i = 0; i < scrollViewContent.transform.childCount; i++) {
            GameObject.Destroy(scrollViewContent.transform.GetChild(i).gameObject);
        }
    }

    private void CreateReportRow(EnemyData.EnemyType enemyType, int enemyAmount) {
        GameObject row = GameObject.Instantiate(reportRowPrefab, scrollViewContent.transform);

        Image icon = row.transform.Find("imgIcon").GetComponent<Image>();
        TMP_Text txtType = row.transform.Find("pnlInfo/pnlTypeAndAmount/txtType").GetComponent<TMP_Text>();
        TMP_Text txtAmount = row.transform.Find("pnlInfo/pnlTypeAndAmount/txtAmount").GetComponent<TMP_Text>();

        EnemyData enemyData = enemyTypeToEnemyData[enemyType];
        txtType.text = enemyData.Name;
        txtAmount.text = enemyAmount.ToString();
        icon.sprite = enemyData.Icon;

        icon.GetComponent<EnemyIcon>().enemyData = enemyData;
    }
}
