namespace DefaultNamespace.GUI {

    using DefaultNamespace.IO;
    using System;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class ControlsPanel : IInitializable {
        InputHandler inputHandler;
        Transform pnlControls;
        GameObject controlPanelPrefab;

        public ControlsPanel(InputHandler inputHandler) {
            this.inputHandler = inputHandler;
        }

        public void Initialize() {
            pnlControls = GameObject.Find("pnlControls").transform;
            controlPanelPrefab = Resources.Load<GameObject>("Prefabs/pnlControl");
            GameObject newControlPanel;
            TMP_Text txtControlName, txtControlHotkey;

            foreach (Command command in Enum.GetValues(typeof(Command))) {
                newControlPanel = GameObject.Instantiate(controlPanelPrefab);
                txtControlName = newControlPanel.transform.Find("txtControlName").GetComponent<TMP_Text>();
                txtControlHotkey = newControlPanel.transform.Find("txtHotkey").GetComponent<TMP_Text>();
                txtControlName.text = command.ToString();
                txtControlHotkey.text = inputHandler.GetHotkeyByCommand(command).ToString();
                newControlPanel.transform.SetParent(pnlControls);
            }
        }
    }
}