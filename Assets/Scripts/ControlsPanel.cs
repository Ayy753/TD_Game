using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class ControlsPanel : IInitializable {
    [Inject] InputHandler inputHandler;
    Transform pnlControls;
    GameObject controlPanelPrefab;

    public void Initialize() {
        pnlControls = GameObject.Find("pnlControls").transform;
        controlPanelPrefab = Resources.Load<GameObject>("Prefabs/pnlControl");
        GameObject newControlPanel;
        TMP_Text txtControlName, txtControlHotkey;

        foreach (InputHandler.Command command in Enum.GetValues(typeof(InputHandler.Command))) {
            newControlPanel = GameObject.Instantiate(controlPanelPrefab);
            txtControlName = newControlPanel.transform.Find("txtControlName").GetComponent<TMP_Text>();
            txtControlHotkey = newControlPanel.transform.Find("txtHotkey").GetComponent<TMP_Text>();
            txtControlName.text = command.ToString();
            txtControlHotkey.text = inputHandler.GetHotkey(command).ToString();
            newControlPanel.transform.SetParent(pnlControls);
        }
    }
}
