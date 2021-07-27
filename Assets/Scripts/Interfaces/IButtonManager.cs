using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles user inputs via buttons
/// </summary>
public interface IButtonManager {
    public void CreateBuildMenuButtons();
    public void EnterBuildMode(StructureData structure);
    public void EnterDemolishMode();
    public void ExitEditMode();
    public void StartNextWave();
    public void IncreaseaSpeed();
    public void DecreaseSpeed();
    public void BindButtonsInScene();
}
