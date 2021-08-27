namespace DefaultNamespace.IO {

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class InputHandler : MonoBehaviour {
        private readonly Dictionary<Command, KeyCode> hotkeyDictionary = new Dictionary<Command, KeyCode>();
        private readonly Dictionary<Command, KeyCode> DEFAULT_HOTKEY_DICTIONARY = new Dictionary<Command, KeyCode>() {
            {Command.TogglePause, KeyCode.Space},
            {Command.FollowTarget, KeyCode.F},
            {Command.DecreaseGameSpeed, KeyCode.Minus},
            {Command.IncreaseGameSpeed, KeyCode.Equals},
            {Command.ToggleMenu, KeyCode.Escape},
        };

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
            LoadUserPrefs();
        }

        private void OnDisable() {
            SaveUserPrefs();
        }

        void Update() {
            PollHotkeysDown();
        }

        private void LoadUserPrefs() {
            foreach (Command command in Enum.GetValues(typeof(Command))) {
                KeyCode defaultKey = DEFAULT_HOTKEY_DICTIONARY[command];
                hotkeyDictionary[command] = (KeyCode)PlayerPrefs.GetInt(command.ToString(), (int)defaultKey);
            }
        }

        private void ResetDefault() {
            foreach (Command command in Enum.GetValues(typeof(Command))) {
                hotkeyDictionary[command] = DEFAULT_HOTKEY_DICTIONARY[command];
            }
            SaveUserPrefs();
        }

        private void SaveUserPrefs() {
            foreach (Command command in Enum.GetValues(typeof(Command))) {
                PlayerPrefs.SetInt(command.ToString(), (int)hotkeyDictionary[command]);
            }
        }

        private void PollHotkeysDown() {
            foreach (Command command in hotkeyDictionary.Keys) {
                if (Input.GetKeyDown(hotkeyDictionary[command])) {
                    OnCommandEntered?.Invoke(command);
                }
            }
        }

        public void AssignHotkeyToCommand(KeyCode hotKey, Command command) {
            hotkeyDictionary[command] = hotKey;
        }

        public KeyCode GetHotkeyByCommand(Command command) {
            return hotkeyDictionary[command];
        }
    }
}
