using UnityEngine;
using Zenject;
using TMPro;

public class GUIController : IGUIManager, IInitializable {
    TMP_Text txtLives, txtGold;
    TMP_Text txtCurrentWave, txtTotalWaves, txtWaveCountdown;
    TMP_Text txtGameSpeed, txtFPS;

    //  gameEnded panel is the parent for the other two panels
    GameObject pnlGameEnded, pnlGameOver, pnlGameWon, pnlMenu;
    GameObject pnlWave;

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

        pnlWave = GameObject.Find("pnlWave");

        txtCurrentWave = GameObject.Find("txtCurrentWaveNum").GetComponent<TMP_Text>();
        txtTotalWaves = GameObject.Find("txtTotalWaveNum").GetComponent<TMP_Text>();
        txtWaveCountdown = GameObject.Find("txtWaveCountdown").GetComponent<TMP_Text>();

        txtGameSpeed = GameObject.Find("txtSpeed").GetComponent<TMP_Text>();
        txtFPS = GameObject.Find("txtFPS").GetComponent<TMP_Text>();

        pnlGameEnded.SetActive(false);
        pnlGameOver.SetActive(false);
        pnlGameWon.SetActive(false);
        pnlMenu.SetActive(false);
    }

    public void UpdateGoldLabel(float gold) {
        txtGold.text = Mathf.RoundToInt(gold).ToString();
    }

    public void UpdateLivesLabel(int lives) {
        txtLives.text = Mathf.RoundToInt(lives).ToString();
    }

    public void ShowGameOverScreen() {
        pnlGameEnded.SetActive(true);
        pnlGameOver.SetActive(true);
    }

    public void ShowGameWonScreen() {
        pnlGameEnded.SetActive(true);
        pnlGameWon.SetActive(true);
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
}
