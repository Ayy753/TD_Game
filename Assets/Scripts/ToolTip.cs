using UnityEngine;
using UnityEngine.UI;
public class ToolTip : MonoBehaviour
{
    private Text tooltipText;
    private RectTransform background;
    void Start()
    {
        tooltipText = transform.Find("Text").GetComponent<Text>();
        background = transform.Find("Background").GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        TrackMouse();
    }

    public void ShowToolTip()
    {
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Follows the mouse cursor
    /// </summary>
    private void TrackMouse()
    {
        transform.position = Input.mousePosition;
        float textPadding = 4f;
        background.sizeDelta = new Vector2(tooltipText.preferredWidth + textPadding * 2f, tooltipText.preferredHeight + textPadding * 2f);
    }

    /// <summary>
    /// Set the targetted tiledata containing the structure's attributes
    /// </summary>
    /// <param name="tile"></param>
    public void SetCurrentTileData(StructureData tile)
    {
        tooltipText.text = tile.ToString();
    }


    public void SetCurrentString(string data)
    {
        tooltipText.text = data;
    }
}
