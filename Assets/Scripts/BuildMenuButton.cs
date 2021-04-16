using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMenuButton : MonoBehaviour, IPointerClickHandler, IDisplayable
{
    private GUIController GUIController;
    private StructureData StructureData;

    private void Start()
    {
        GUIController = GameManager.Instance.GUIController;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StructureData != null)
        {
            GUIController.EnterBuildMode(StructureData);
        }
        else
        {
            throw new System.Exception("Build menu button does not have a structure class associated with it");
        }
    }

    /// <summary>
    /// Initialize button with the StructureData it represents
    /// </summary>
    /// <param name="StructureData"></param>
    public void SetStructureData(StructureData structureData)
    {
        StructureData = structureData;
    }

    public string GetDisplayText()
    {
        return string.Format(StructureData.ToString());
    }
}
