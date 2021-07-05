using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGUIManager{
    public void CreateDynamicButtons();
    public void EnterBuildMode(StructureData structure);
    public void EnterDemolishMode();
    public void ExitEditMode();
    public void StartNextWave();
    public void UpdateGoldLabel(float gold);
    public void UpdateLivesLabel(int lives);
    public void ShowGameOverScreen();
    public void ShowGameWonScreen();
    public void ShowMenu();
    public void HideMenu();
    public void UpdateWaveNumber(int current, int total);
    public void UpdateWaveCountdown(int secondsLeft);
    public void UpdateSpeedPanel(float speed);
    public void IncreaseaSpeed();
    public void DecreaseSpeed();
    public void UpdateFPSCounter(int fps);
}
