using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class GUIController : MonoBehaviour, IGUIManager {

    [Inject] BuildManager buildManager;
    [Inject] WaveManager waveManager;
    [Inject] GameManager gameManager;

    TMP_Text txtLives, txtGold;
    TMP_Text txtCurrentWave, txtTotalWaves, txtWaveCountdown;
    TMP_Text txtGameSpeed;

    //  gameEnded panel is the parent for the other two panels
    GameObject pnlGameEnded, pnlGameOver, pnlGameWon;
    GameObject pnlWave;

    public void Awake() {
        CreateDynamicButtons();
        
        txtLives = GameObject.Find("txtLivesVal").GetComponent<TMP_Text>();
        txtGold = GameObject.Find("txtGoldVal").GetComponent<TMP_Text>();

        pnlGameEnded = GameObject.Find("pnlGameEnded");
        pnlGameOver = GameObject.Find("pnlGameOver");
        pnlGameWon = GameObject.Find("pnlGameWon");

        pnlWave = GameObject.Find("pnlWave");

        txtCurrentWave = GameObject.Find("txtCurrentWaveNum").GetComponent<TMP_Text>();
        txtTotalWaves = GameObject.Find("txtTotalWaveNum").GetComponent<TMP_Text>();
        txtWaveCountdown = GameObject.Find("txtWaveCountdown").GetComponent<TMP_Text>();

        txtGameSpeed = GameObject.Find("txtSpeed").GetComponent<TMP_Text>();

        pnlGameEnded.SetActive(false);
        pnlGameOver.SetActive(false);
        pnlGameWon.SetActive(false);
    }

    /// <summary>
    /// Populates build menu with structure buttons
    /// </summary>
    public void CreateDynamicButtons() {
        GameObject structureBuildBtnPrefab = Resources.Load<GameObject>("Prefabs/NewBuildMenuButton");
        StructureData[] structureDatas = Resources.LoadAll<StructureData>("ScriptableObjects/TileData/StructureData");
        GameObject scrollViewContentBox = GameObject.Find("Scroll View/Viewport/Content");

        foreach (StructureData structure in structureDatas) {
            GameObject newButton = GameObject.Instantiate(structureBuildBtnPrefab);
            newButton.transform.SetParent(scrollViewContentBox.transform);
            newButton.GetComponent<Image>().sprite = structure.Icon;
            newButton.name = structure.Name;

            //  Not sure why but the scale gets messed up, so this is a fix
            newButton.transform.localScale = new Vector3(1, 1, 1);

            newButton.GetComponent<BuildMenuButton>().Initialize(structure, this);
        }
    }

    public void UpdateGoldLabel(float gold) {
        txtGold.text = Mathf.RoundToInt(gold).ToString();
    }

    public void UpdateLivesLabel(int lives) {
        txtLives.text = Mathf.RoundToInt(lives).ToString();
    }
    
    public void EnterBuildMode(StructureData structure) {
        buildManager.EnterBuildMode(structure);
    }

    public void EnterDemolishMode() {
        buildManager.EnterDemolishMode();
    }

    public void ExitEditMode() {
        buildManager.ExitBuildMode();
    }

    public void StartNextWave() {
        waveManager.StartNextWave();
    }

    public void ShowGameOverScreen() {
        pnlGameEnded.SetActive(true);
        pnlGameOver.SetActive(true);
    }

    public void ShowGameWonScreen() {
        pnlGameEnded.SetActive(true);
        pnlGameWon.SetActive(true);
    }
    
    public void Restart() {
        gameManager.Restart();
    }

    public void ExitGame() {
        gameManager.ExitGame();
    }

    public void UpdateWaveNumber(int current, int total) {
        txtCurrentWave.text = current.ToString();
        txtTotalWaves.text = total.ToString();
    }

    public void UpdateWaveCountdown(int secondsLeft) {
        txtWaveCountdown.text = secondsLeft.ToString();
    }

    public void UpdateSpeedPanel(float speed) {
        txtGameSpeed.text = speed.ToString();
    }

    public void IncreaseaSpeed() {
        gameManager.IncreaseGameSpeed();
    }

    public void DecreaseSpeed() {
        gameManager.DecreaseGameSpeed();
    }
}


