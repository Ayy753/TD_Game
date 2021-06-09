using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TargetManager: IInitializable, IDisposable{

    private StatusPanel statusPanel;
    private TowerPanel towerPanel;

    public TargetManager(StatusPanel statusPanel, TowerPanel towerPanel) {
        this.statusPanel = statusPanel;
        this.towerPanel = towerPanel;
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
            towerPanel.UpdateTowerPanel(tower);
        }
        else if (unit != null) {
            statusPanel.UpdateStatusPanel(unit.GetStatus());
        }
    }
}
