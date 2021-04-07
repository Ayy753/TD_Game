using UnityEngine;
using UnityEngine.UI;
public class ToolTip : MonoBehaviour
{
    private Text tooltipText;
    private RectTransform tooltipBackground;
    void Start()
    {
        tooltipText = transform.Find("Text").GetComponent<Text>();
        tooltipBackground = transform.Find("Background").GetComponent<RectTransform>();
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
        tooltipBackground.sizeDelta = new Vector2(tooltipText.preferredWidth + textPadding * 2f, tooltipText.preferredHeight + textPadding * 2f);
    }

    public void SetCurrentString(string data)
    {
        tooltipText.text = data;
    }
}
