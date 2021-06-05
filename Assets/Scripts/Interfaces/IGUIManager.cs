using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGUIManager{
    public void CreateDynamicButtons();
    public void EnterBuildMode(StructureData structure);
    public void EnterDemolishMode();
    public void ExitEditMode();
    public void StartNextWave();
}
