using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BuildMenuButton : MonoBehaviour, IPointerClickHandler, IDisplayable {

    [Inject] private IGUIManager guiController;
    StructureData structureData;

    private void Start() {
        Debug.Log(structureData.ToString());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (structureData != null)
        {
            guiController.EnterBuildMode(structureData);
        }
        else
        {
            throw new System.Exception("Build menu button does not have a structure class associated with it");
        }
    }

    public string GetDisplayText()
    {
        return string.Format(structureData.ToString());
    }

    /// <summary>
    /// Initialize button with the StructureData it represents
    /// </summary>
    public void Initialize(StructureData structureData, GUIController guiController) {
        this.structureData = structureData;
        this.guiController = guiController;
    }
}
