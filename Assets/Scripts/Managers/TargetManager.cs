using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TargetManager: IInitializable, IDisposable{

    private StatusPanel statusPanel;
    private TowerPanel towerPanel;

    private GameObject targetIndicator;
    private GameObject currentTarget;

    public TargetManager(StatusPanel statusPanel, TowerPanel towerPanel) {
        this.statusPanel = statusPanel;
        this.towerPanel = towerPanel;
    }

    public void Initialize() {
        MouseManager.OnGameObjectClicked += HandleGameObjectClicked;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/TargetIndicator");
        targetIndicator = GameObject.Instantiate(prefab);
    }

    public void Dispose() {
        MouseManager.OnGameObjectClicked -= HandleGameObjectClicked;
    }

    private void HandleGameObjectClicked(GameObject gameObject) {
        Tower tower = gameObject.GetComponent<Tower>();
        IUnit unit = gameObject.GetComponentInChildren<IUnit>();

        if (tower != null) {
            towerPanel.TargetTower(tower);
            targetIndicator.transform.SetParent(tower.transform);
            targetIndicator.transform.localPosition = Vector3.zero;
        }
        else if (unit != null) {
            statusPanel.TargetUnit(unit);
            targetIndicator.transform.SetParent(unit.GetTransform());
            targetIndicator.transform.localPosition = Vector3.zero;
        }
    }
}
