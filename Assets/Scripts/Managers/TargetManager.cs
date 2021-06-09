using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TargetManager: IInitializable, IDisposable{

    private StatusPanel statusPanel;

    public TargetManager(StatusPanel statusPanel) {
        this.statusPanel = statusPanel;
    }

    public void Initialize() {
        MouseManager.OnGameObjectClicked += HandleGameObjectClicked;
    }

    public void Dispose() {
        MouseManager.OnGameObjectClicked -= HandleGameObjectClicked;
    }

    private void HandleGameObjectClicked(GameObject gameObject) {
        Tower tower = gameObject.GetComponent<Tower>();
        IUnit unit = gameObject.GetComponentInChildren<IUnit>();

        if (tower != null) {
            //  TODO implement
        }
        else if (unit != null) {
            statusPanel.UpdateStatusPanel(unit.GetStatus());
        }
    }
}
