namespace DefaultNamespace.GUI {

    using UnityEngine;
    using UnityEngine.UI;
    public class ToolTip : MonoBehaviour {
        private Text tooltipText;
        private RectTransform tooltipBackground;
        private RectTransform canvasRect;
        private RectTransform tooltipTransform;

        private void Start() {
            tooltipText = transform.Find("Text").GetComponent<Text>();
            tooltipBackground = transform.Find("Background").GetComponent<RectTransform>();
            canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
            tooltipTransform = transform.GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        private void Update() {
            TrackMouse();
        }

        /// <summary>
        /// Follows the mouse cursor
        /// </summary>
        private void TrackMouse() {
            //  Clamp tooltip within bounds of screen
            Vector3 cursorPos = Input.mousePosition / canvasRect.localScale.x;
            tooltipTransform.anchoredPosition = new Vector3(Mathf.Clamp(cursorPos.x, 0, canvasRect.rect.width - tooltipBackground.rect.width), Mathf.Clamp(cursorPos.y, 0, canvasRect.rect.height - tooltipBackground.rect.height));
        }

        /// <summary>
        /// Sets the current tooltip text
        /// </summary>
        /// <param name="data"></param>
        private void SetCurrentString(string data) {
            tooltipText.text = data;
            //  Calculate the tooltip background size based on text size and padding
            float textPadding = 4f;
            tooltipBackground.sizeDelta = new Vector2(tooltipText.preferredWidth + textPadding * 4f, tooltipText.preferredHeight + textPadding * 4f);
        }

        /// <summary>
        /// Disables the tooltip
        /// </summary>
        public void HideToolTip() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Enables the tooltip
        /// </summary>
        public void ShowToolTip(IDisplayable displayable) {
            SetCurrentString(displayable.GetDisplayText());
            TrackMouse();
            gameObject.SetActive(true);
        }
    }
}
