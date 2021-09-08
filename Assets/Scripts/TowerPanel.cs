namespace DefaultNamespace.GUI {

    using DefaultNamespace;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class TowerPanel : IInitializable {
        readonly IWallet wallet;
        readonly BuildManager buildManager;

        GameObject pnlTowerPanel;
        TMP_Text txtName, txtRange, txtDamage, txtReloadTime, txtProjectileType, txtSellValue, txtTowerDescription;
        Button btnSell;
        TMP_Dropdown targetModeDropdown;
        Tower currentlySelectedTower;

        public TowerPanel(IWallet wallet, BuildManager buildManager) {
            this.wallet = wallet;
            this.buildManager = buildManager;
        }

        public void Initialize() {
            Debug.Log("initializing tower panel");

            pnlTowerPanel = GameObject.Find("pnlTower");

            txtName = GameObject.Find("txtTowerName").GetComponent<TMP_Text>();
            txtRange = GameObject.Find("txtRangeVal").GetComponent<TMP_Text>();
            txtDamage = GameObject.Find("txtDamageVal").GetComponent<TMP_Text>();
            txtReloadTime = GameObject.Find("txtReloadTimeVal").GetComponent<TMP_Text>();
            txtProjectileType = GameObject.Find("txtProjectileTypeVal").GetComponent<TMP_Text>();
            txtTowerDescription = GameObject.Find("txtTowerDescription").GetComponent<TMP_Text>();

            btnSell = GameObject.Find("btnSell").GetComponent<Button>();

            targetModeDropdown = GameObject.Find("dropdownTargetMode").GetComponent<TMP_Dropdown>();
            txtSellValue = GameObject.Find("txtSellVal").GetComponent<TMP_Text>();

            btnSell.onClick.AddListener(delegate { SellTower(); });
            targetModeDropdown.onValueChanged.AddListener(delegate { TargetModeChanged(); });

            pnlTowerPanel.SetActive(false);
        }

        private void TargetModeChanged() {
            int index = targetModeDropdown.value;
            string optionText = targetModeDropdown.options[index].text;
            switch (optionText) {
                case "Closest":
                    SetTargetMode(Tower.TargetMode.Closest);
                    break;
                case "Furthest":
                    SetTargetMode(Tower.TargetMode.Furthest);
                    break;
                case "Random":
                    SetTargetMode(Tower.TargetMode.Random);
                    break;
                case "Lowest health":
                    SetTargetMode(Tower.TargetMode.LowestHealth);
                    break;
                case "Highest health":
                    SetTargetMode(Tower.TargetMode.HighestHealth);
                    break;
            }
        }

        private void UpdateTowerPanel() {
            TowerData towerData = currentlySelectedTower.TowerData;

            txtName.text = towerData.Name;
            txtRange.text = towerData.Range.ToString();
            txtDamage.text = towerData.EffectGroup.GetTotalDamage().ToString();
            txtReloadTime.text = towerData.EffectGroup.Cooldown.ToString();
            txtProjectileType.text = towerData.EffectGroup.Name;

            string descriptionText = "<b>Description</b>:\n" + towerData.Description + "\n\n<b>Projectile Effects</b>: \n" + towerData.EffectGroup.GetEffectInfo();
            txtTowerDescription.text = descriptionText;

            int sellValue = Mathf.RoundToInt(towerData.Cost * wallet.GetResellPercentageInDecimal());
            txtSellValue.text = sellValue.ToString();

            UpdateTargetDropdownIndex();
        }

        private void UpdateTargetDropdownIndex() {
            switch (currentlySelectedTower.CurrentTargetMode) {
                case Tower.TargetMode.Closest:
                    targetModeDropdown.value = GetDropdwonIndexByName("Closest");
                    break;
                case Tower.TargetMode.Furthest:
                    targetModeDropdown.value = GetDropdwonIndexByName("Furthest");
                    break;
                case Tower.TargetMode.Random:
                    targetModeDropdown.value = GetDropdwonIndexByName("Random");
                    break;
                case Tower.TargetMode.LowestHealth:
                    targetModeDropdown.value = GetDropdwonIndexByName("Lowest health");
                    break;
                case Tower.TargetMode.HighestHealth:
                    targetModeDropdown.value = GetDropdwonIndexByName("Highest health");
                    break;
            }
        }

        private int GetDropdwonIndexByName(string name) {
            for (int i = 0; i < targetModeDropdown.options.Count; i++) {
                if (targetModeDropdown.options[i].text == name) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Sells the currently selected tower
        /// </summary>
        private void SellTower() {
            if (currentlySelectedTower != null) {
                buildManager.SellTower(currentlySelectedTower);
                ClearTarget();
            }
            else {
                Debug.LogError("It should be impossible to call this function when no tower is selected");
            }
        }

        public void SetTargetMode(Tower.TargetMode targetMode) {
            currentlySelectedTower.ChangeTargetMode(targetMode);
        }

        public void TargetTower(Tower tower) {
            currentlySelectedTower = tower;
            pnlTowerPanel.SetActive(true);
            UpdateTowerPanel();
        }

        public void ClearTarget() {
            currentlySelectedTower = null;
            pnlTowerPanel.SetActive(false);
        }
    }
}
