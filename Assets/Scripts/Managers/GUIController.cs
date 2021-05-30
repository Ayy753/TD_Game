using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class GUIController : MonoBehaviour {

    [Inject] BuildManager buildManager;

    public void Awake() {
        PopulateScrollView();
    }

    /// <summary>
    /// Populates build menu with structure buttons
    /// </summary>
    private void PopulateScrollView() {
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

    internal void SpawnFloatingText(Vector3 vector3, string v, Color yellow) {
        throw new NotImplementedException();
    }

    internal void SpawnFloatingTextAtCursor(string v, Color red) {
        throw new NotImplementedException();
    }

    internal void SpawnFloatingTextInCenter(string v, Color yellow) {
        throw new NotImplementedException();
    }
    
    public void EnterBuildMode(StructureData structure) {
        buildManager.EnterBuildMode(structure);
    }

    public void ExitBuildMode() {
        buildManager.ExitBuildMode();
    }

}
