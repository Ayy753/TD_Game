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

    public delegate void KeyPressed(Command command);
    public static event KeyPressed OnCommandEntered;

    public enum Command {
        TogglePause,
        FocusTarget,
        SlowGameSpeed,
        IncreaseGameSpeed
    }

    public void SetControl(Command command, KeyCode key) {
        switch (command) {
            case Command.TogglePause:
                togglePause = key;
                break;
            case Command.FocusTarget:
                focusTarget = key;
                break;
            case Command.SlowGameSpeed:
                slowGameSpeed = key;
                break;
            case Command.IncreaseGameSpeed:
                increaseGameSpeed = key;
                break;
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
                OnCommandEntered.Invoke(Command.SlowGameSpeed);
            }
            else if (Input.GetKeyDown(increaseGameSpeed)) {
                OnCommandEntered.Invoke(Command.IncreaseGameSpeed);
            }
        }
    }
}
