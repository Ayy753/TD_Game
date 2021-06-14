using System;
using UnityEngine;
using Zenject;

public class TargetManager: IInitializable, IDisposable{

    private StatusPanel statusPanel;
    private TowerPanel towerPanel;

    private GameObject targetIndicator;

    //private Itargetable target;
    private Itargetable target;

    public TargetManager(StatusPanel statusPanel, TowerPanel towerPanel) {
        this.statusPanel = statusPanel;
        this.towerPanel = towerPanel;
    }

    public void Initialize() {
        MouseManager.OnGameObjectClicked += HandleGameObjectClicked;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/TargetIndicator");
        targetIndicator = GameObject.Instantiate(prefab);
        targetIndicator.SetActive(false);
    }

    public void Dispose() {
        MouseManager.OnGameObjectClicked -= HandleGameObjectClicked;
    }

    private void HandleGameObjectClicked(GameObject gameObject) {

        if (gameObject.GetComponent<Itargetable>() != null) {
            //  If there is currently a target, untarget it first
            if (target != null) {
                Untarget();
            }
            Target(gameObject);
        }
    }

    /// <summary>
    /// Subscribes to target's events and populates target panel
    /// </summary>
    /// <param name="gameObject"></param>
    private void Target(GameObject gameObject) {
        Tower tower = gameObject.GetComponent<Tower>();
        Unit unit = gameObject.GetComponentInChildren<Unit>();

        if (tower != null) {
            towerPanel.TargetTower(tower);
            target = tower;
        }
        else if (unit != null) {
            statusPanel.TargetUnit(unit);
            target = unit;
        }

        target.TargetDisabled += HandleTargetDisabled;

        targetIndicator.SetActive(true);
        targetIndicator.transform.SetParent(gameObject.transform);
        targetIndicator.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Unsubscribes from target's events and clears target panel
    /// </summary>
    private void Untarget() {
        target.TargetDisabled -= HandleTargetDisabled;

        if (target.GetType() == typeof(Tower)) {
            towerPanel.ClearTarget();
        }
        else if (target.GetType().IsSubclassOf(typeof(Unit))) {
            statusPanel.ClearTarget();
        }

        target = null;

        targetIndicator.transform.SetParent(null);
        targetIndicator.SetActive(false);
    }

    private void HandleTargetDisabled(object sender, EventArgs eventArgs) {
        Untarget();
    }
}
