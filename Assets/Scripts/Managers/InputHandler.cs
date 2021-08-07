using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour{
    private Dictionary<Command, KeyCode> commandToHotkeyDictionary;

    public delegate void KeyPressed(Command command);
    public static event KeyPressed OnCommandEntered;

    public enum Command {
        TogglePause,
        FollowTarget,
        DecreaseGameSpeed,
        IncreaseGameSpeed,
        ToggleMenu
    }

    private void OnEnable() {
        //  TODO: Load this from a pref file
        commandToHotkeyDictionary = new Dictionary<Command, KeyCode>() {
            {Command.TogglePause, KeyCode.Space},
            {Command.FollowTarget, KeyCode.F},
            {Command.DecreaseGameSpeed, KeyCode.Minus},
            {Command.IncreaseGameSpeed, KeyCode.Equals},
            {Command.ToggleMenu, KeyCode.Escape},
        };
    }
    
    public void AssignKeyToCommand(KeyCode hotKey, Command command) {
        commandToHotkeyDictionary[command] = hotKey;
    }

    public KeyCode GetHotkeyByCommand(Command command) {
        return commandToHotkeyDictionary[command];
    }

    void Update(){
        if (OnCommandEntered != null) {
            foreach (Command command in commandToHotkeyDictionary.Keys) {
                if (Input.GetKeyDown(commandToHotkeyDictionary[command])) {
                    OnCommandEntered.Invoke(command);
                }
            }
        }
    }
}
