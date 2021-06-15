using UnityEngine;

public interface IMessageSystem{
    /// <summary>
    /// Displays message at center
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void DisplayMessage(string message, Color color, float textSize = 0.5f);

    /// <summary>
    /// Displays message at position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void DisplayMessageAt(Vector3 position, string message, Color color, float textSize = 0.5f);

    /// <summary>
    /// Displays message at cursor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void DisplayMessageAtCursor(string message, Color color, float textSize = 0.5f);
}
