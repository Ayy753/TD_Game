using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class GUIController : MonoBehaviour, IGUIManager {

    [Inject] BuildManager buildManager;
    [Inject] WaveManager waveManager;

    public void Awake() {
        CreateDynamicButtons();

        Debug.Log(buildManager == null);
        Debug.Log(waveManager == null);
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
}


