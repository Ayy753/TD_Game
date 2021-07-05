using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that listens for keyput input and fires events
/// </summary>
public class InputHandler : MonoBehaviour{
    private KeyCode togglePause = KeyCode.Space;
    private KeyCode focusTarget = KeyCode.F;
    private KeyCode slowGameSpeed = KeyCode.Minus;
    private KeyCode increaseGameSpeed = KeyCode.Equals;
    private KeyCode toggleMenu = KeyCode.Escape;

    public delegate void KeyPressed(Command command);
    public static event KeyPressed OnCommandEntered;

    public enum Command {
        TogglePause,
        FocusTarget,
        DecreaseGameSpeed,
        IncreaseGameSpeed,
        ToggleMenu
    }

    /// <summary>
    /// Assign a hotkey to a command
    /// </summary>
    /// <param name="command"></param>
    /// <param name="key"></param>
    public void SetControl(Command command, KeyCode key) {
        switch (command) {
            case Command.TogglePause:
                togglePause = key;
                break;
            case Command.FocusTarget:
                focusTarget = key;
                break;
            case Command.DecreaseGameSpeed:
                slowGameSpeed = key;
                break;
            case Command.IncreaseGameSpeed:
                increaseGameSpeed = key;
                break;
            case Command.ToggleMenu:
                toggleMenu = key;
                break;
        }
    }

    /// <summary>
    /// Get the hotkey assigned to a command
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public KeyCode GetHotkey(Command command) {
        switch (command) {
            case Command.TogglePause:
                return togglePause;
            case Command.FocusTarget:
                return focusTarget;
            case Command.DecreaseGameSpeed:
                return slowGameSpeed;
            case Command.IncreaseGameSpeed:
                return increaseGameSpeed;
            case Command.ToggleMenu:
                return toggleMenu;
            default:
                return KeyCode.None;
        }
    }

    void Update(){
        if (OnCommandEntered != null) {
            if (Input.GetKeyDown(togglePause)) {
                OnCommandEntered.Invoke(Command.TogglePause);
            } 
            else if (Input.GetKeyDown(focusTarget)) {
                OnCommandEntered.Invoke(Command.FocusTarget);
            }
            else if (Input.GetKeyDown(slowGameSpeed)) {
                OnCommandEntered.Invoke(Command.DecreaseGameSpeed);
            }
            else if (Input.GetKeyDown(increaseGameSpeed)) {
                OnCommandEntered.Invoke(Command.IncreaseGameSpeed);
            }
            else if (Input.GetKeyDown(toggleMenu)) {
                OnCommandEntered.Invoke(Command.ToggleMenu);
            }
        }
    }
}
