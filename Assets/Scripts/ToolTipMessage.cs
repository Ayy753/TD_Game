namespace DefaultNamespace.GUI {
    using DefaultNamespace;
    using UnityEngine;

    /// <summary>
    /// Attach to any Canvas UI element
    /// </summary>
    public class ToolTipMessage : MonoBehaviour, IDisplayable {
        [SerializeField] private string message = "Default Text";

        public string GetDisplayText() {
            return message;
        }
    }
}
