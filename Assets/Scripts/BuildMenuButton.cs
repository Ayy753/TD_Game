using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private GUIController GUIController;
    private StructureData StructureData;

    private void Start()
    {
        GUIController = GameManager.Instance.GUIController;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GUIController.ShowToolTip();
        GUIController.SetCurrentStructureData(StructureData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GUIController.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StructureData != null)
        {
            GUIController.BuildStructure(StructureData);
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
}
