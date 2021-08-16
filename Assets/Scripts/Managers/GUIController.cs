using UnityEngine;
using Zenject;
using TMPro;

public class GUIController : IGUIManager, IInitializable {
    TMP_Text txtLives, txtGold;
    TMP_Text txtCurrentWave, txtTotalWaves, txtWaveCountdown, txtEnemiesRemaining;
    TMP_Text txtGameSpeed, txtFPS;

    //  gameEnded panel is the parent for the other two panels
    GameObject pnlGameEnded, pnlGameOver, pnlGameWon, pnlMenu, pnlBuildMenu, pnlPause;
    GameObject imgBuildMenuLock;

    public GUIController() {
        Debug.Log("GUIController constuctor");
    }

    public void Initialize() {
        txtLives = GameObject.Find("txtLivesVal").GetComponent<TMP_Text>();
        txtGold = GameObject.Find("txtGoldVal").GetComponent<TMP_Text>();

        pnlGameEnded = GameObject.Find("pnlGameEnded");
        pnlGameOver = GameObject.Find("pnlGameOver");
        pnlGameWon = GameObject.Find("pnlGameWon");
        pnlMenu = GameObject.Find("pnlMenu");
        pnlBuildMenu = GameObject.Find("pnlBuildMenu");
        imgBuildMenuLock = GameObject.Find("imgLock");
        pnlPause = GameObject.Find("pnlPause");

        txtCurrentWave = GameObject.Find("txtCurrentWaveNum").GetComponent<TMP_Text>();
        txtTotalWaves = GameObject.Find("txtTotalWaveNum").GetComponent<TMP_Text>();
        txtWaveCountdown = GameObject.Find("txtWaveCountdown").GetComponent<TMP_Text>();
        txtEnemiesRemaining = GameObject.Find("txtEnemiesRemaining").GetComponent<TMP_Text>();

        txtGameSpeed = GameObject.Find("txtSpeed").GetComponent<TMP_Text>();
        txtFPS = GameObject.Find("txtFPS").GetComponent<TMP_Text>();

        pnlGameEnded.SetActive(false);
        pnlGameOver.SetActive(false);
        pnlGameWon.SetActive(false);
        pnlMenu.SetActive(false);
        imgBuildMenuLock.SetActive(false);
        pnlPause.SetActive(false);
    }

    public void UpdateGoldLabel(float gold) {
        txtGold.text = Mathf.RoundToInt(gold).ToString();
    }

    public void UpdateLivesLabel(int lives) {
        txtLives.text = Mathf.RoundToInt(lives).ToString();
    }

    public void ShowGameOverScreen() {
        pnlMenu.SetActive(false);
        pnlGameEnded.SetActive(true);
        pnlGameOver.SetActive(true);
    }

    public void ShowGameWonScreen() {
        pnlMenu.SetActive(false);
        pnlGameEnded.SetActive(true);
        pnlGameWon.SetActive(true);
    }

    public void HideGameEndedPanel() {
        pnlGameWon.SetActive(false);
        pnlGameOver.SetActive(false);
        pnlGameEnded.SetActive(false);
    }

    public void ShowMenu() {
        pnlGameEnded.SetActive(true);
        pnlMenu.SetActive(true);
    }

    public void HideMenu() {
        pnlGameEnded.SetActive(false);
        pnlMenu.SetActive(false);
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

    public void UpdateFPSCounter(int fps) {
        txtFPS.text = fps.ToString();
    }

    public void UpdateEnemiesRemainingLabel(int enemiesRemaining) {
        txtEnemiesRemaining.text = enemiesRemaining.ToString();
    }

    public void LockBuildMenu() {
        imgBuildMenuLock.SetActive(true);
    }

    public void UnlockBuildMenu() {
        imgBuildMenuLock.SetActive(false);
    }

    public void ShowPausePanel() {
        pnlPause.SetActive(true);
    }

    public void HidePausePanel() {
        pnlPause.SetActive(false);
    }
}
