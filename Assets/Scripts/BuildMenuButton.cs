using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private GUIController GUIController;
    private StructureData StructureData;

    private GameObject toolTip;

    private void Awake()
    {
        toolTip = GameObject.Find("ToolTip");
    }

    private void Start()
    {
        GUIController = GameManager.Instance.GUIController;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
        toolTip.GetComponent<ToolTip>().SetCurrentTile(StructureData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
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
    /// Associate this dynamic button with a class of structure
    /// </summary>
    /// <param name="structureData"></param>
    public void SetStructureData(StructureData structureData)
    {
        StructureData = structureData;
    }
}
