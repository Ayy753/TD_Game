using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages panels and label displays 
/// </summary>
public interface IGUIManager{
    public void UpdateGoldLabel(float gold);
    public void UpdateLivesLabel(int lives);
    public void ShowGameOverScreen();
    public void ShowGameWonScreen();
    public void ShowMenu();
    public void HideMenu();
    public void UpdateWaveNumber(int current, int total);
    public void UpdateWaveCountdown(int secondsLeft);
    public void UpdateSpeedPanel(float speed);
    public void UpdateFPSCounter(int fps);
}

