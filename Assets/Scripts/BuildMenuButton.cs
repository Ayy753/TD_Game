using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMenuButton : MonoBehaviour, IPointerClickHandler, IDisplayable {
    private IButtonManager buttonManager;
    private StructureData structureData;

    private void Start() {
        Debug.Log(structureData.ToString());
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (structureData != null) {
            buttonManager.EnterBuildMode(structureData);
        }
        else {
            throw new System.Exception("Build menu button does not have a structure class associated with it");
        }
    }

    public string GetDisplayText() {
        return string.Format(structureData.ToString());
    }

    /// <summary>
    /// Initialize button with the StructureData it represents
    /// </summary>
    public void Initialize(StructureData structureData, IButtonManager buttonManager) {
        this.structureData = structureData;
        this.buttonManager = buttonManager;
    }
}
