namespace DefaultNamespace {
    using DefaultNamespace.GUI;
    using System;
    using UnityEngine;
    using Zenject;

    public class TargetManager : IInitializable, IDisposable {
        private StatusPanel statusPanel;
        private TowerPanel towerPanel;
        private TotemPanel totemPanel;
        private GameObject targetIndicator;
        private Itargetable target;

        public TargetManager(StatusPanel statusPanel, TowerPanel towerPanel, TotemPanel totemPanel) {
            this.statusPanel = statusPanel;
            this.towerPanel = towerPanel;
            this.totemPanel = totemPanel;
        }

        public void Initialize() {
            MouseManager.OnGameObjectClicked += HandleGameObjectClicked;
            MouseManager.OnRightMouseUp += HandleRightMouseUp;

            GameObject prefab = Resources.Load<GameObject>("Prefabs/TargetIndicator");
            targetIndicator = GameObject.Instantiate(prefab);
            targetIndicator.SetActive(false);
        }

        public void Dispose() {
            MouseManager.OnGameObjectClicked -= HandleGameObjectClicked;
            MouseManager.OnRightMouseUp -= HandleRightMouseUp;
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
        /// Untargets when user rightclicks
        /// </summary>
        private void HandleRightMouseUp() {
            if (target != null) {
                Untarget();
            }
        }

        /// <summary>s
        /// Subscribes to target's events and populates target panel
        /// </summary>
        /// <param name="gameObject"></param>
        private void Target(GameObject gameObject) {
            Tower tower = gameObject.GetComponent<Tower>();
            IUnit unit = gameObject.GetComponentInChildren<IUnit>();
            Totem totem = gameObject.GetComponent<Totem>();

            if (tower != null) {
                towerPanel.TargetTower(tower);
                target = tower;
            }
            else if (unit != null) {
                statusPanel.TargetUnit(unit);
                target = unit;
            }
            else if (totem != null) {
                totemPanel.TargetTotem(totem);
                target = totem;
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

            if (target is Tower) {
                towerPanel.ClearTarget();
            }
            else if (target is IUnit) {
                statusPanel.ClearTarget();
            }
            else if (target is Totem) {
                totemPanel.ClearTarget();
            }

            target = null;

            targetIndicator.transform.SetParent(null);
            targetIndicator.SetActive(false);
        }

        private void HandleTargetDisabled(object sender, EventArgs eventArgs) {
            Untarget();
        }

        public Itargetable GetTarget() {
            return target;
        }
    }
}
