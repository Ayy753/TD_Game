namespace DefaultNamespace.GUI {

    using DefaultNamespace.IO;
    using System;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class ControlsPanel : IInitializable {
        readonly InputHandler inputHandler;

        public ControlsPanel(InputHandler inputHandler) {
            this.inputHandler = inputHandler;
        }

        public void Initialize() {
            Transform pnlControls = GameObject.Find("pnlControls").transform;
            GameObject controlPanelPrefab = Resources.Load<GameObject>("Prefabs/pnlControl");
            GameObject newControlPanel;
            TMP_Text txtControlName, txtControlHotkey;

            foreach (Command command in Enum.GetValues(typeof(Command))) {
                newControlPanel = GameObject.Instantiate(controlPanelPrefab);
                txtControlName = newControlPanel.transform.Find("txtControlName").GetComponent<TMP_Text>();
                txtControlHotkey = newControlPanel.transform.Find("txtHotkey").GetComponent<TMP_Text>();
                txtControlName.text = command.ToString();
                txtControlHotkey.text = inputHandler.GetHotkeyByCommand(command).ToString();
                newControlPanel.transform.SetParent(pnlControls);
                newControlPanel.transform.localScale = Vector3.one;
            }
        }
    }
}