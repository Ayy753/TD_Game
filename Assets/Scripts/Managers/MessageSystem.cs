using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem : IMessageSystem {
    private ObjectPool objectPool;

    public MessageSystem(ObjectPool objectPool) {
        this.objectPool = objectPool;
    }

    /// <summary>
    /// Displays message at center
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void DisplayMessage(string message, Color color) {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        DisplayMessageAt(pos, message, color);
    }

    /// <summary>
    /// Displays message at position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void DisplayMessageAt(Vector3 position, string message, Color color) {
        objectPool.CreateFloatingText(position, message, color);
    }

    /// <summary>
    /// Displays message at cursor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void DisplayMessageAtCursor(string message, Color color) {
        DisplayMessageAt(Camera.main.ScreenToWorldPoint(Input.mousePosition), message, color);
    }
}
