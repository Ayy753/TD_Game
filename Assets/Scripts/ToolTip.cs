using UnityEngine;
using UnityEngine.UI;
public class ToolTip : MonoBehaviour
{
    private Text tooltipText;
    private RectTransform tooltipBackground;
    private RectTransform canvasRect;
    private RectTransform rectTransform;
    void Start()
    {
        tooltipText = transform.Find("Text").GetComponent<Text>();
        tooltipBackground = transform.Find("Background").GetComponent<RectTransform>();
        canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        rectTransform = transform.GetComponent<RectTransform>();
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
        //  Calculate the tooltip background size based on text size and padding
        float textPadding = 4f;
        tooltipBackground.sizeDelta = new Vector2(tooltipText.preferredWidth + textPadding * 2f, tooltipText.preferredHeight + textPadding * 2f);

        //  Clamp tooltip within bounds of screen
        Vector3 cursorPos = Input.mousePosition / canvasRect.localScale.x;
        rectTransform.anchoredPosition = new Vector3(Mathf.Clamp(cursorPos.x, 0, canvasRect.rect.width - tooltipBackground.rect.width), Mathf.Clamp(cursorPos.y, 0, canvasRect.rect.height - tooltipBackground.rect.height));
    }

    public void SetCurrentString(string data)
    {
        tooltipText.text = data;
    }
}