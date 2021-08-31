namespace DefaultNamespace.GUI {

    using DefaultNamespace;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class TowerPanel : IInitializable {
        readonly IWallet wallet;
        readonly BuildManager buildManager;

        GameObject pnlTowerPanel;
        TMP_Text txtName, txtRange, txtDamage, txtReloadTime, txtProjectileType, txtSellValue, txtTowerDescription;
        Button btnClosest, btnFurthest, btnLowHP, btnHighHP, btnRandom, btnSell;
        Button[] targetButtons;
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

            btnClosest = GameObject.Find("btnClosest").GetComponent<Button>();
            btnFurthest = GameObject.Find("btnFurthest").GetComponent<Button>();
            btnLowHP = GameObject.Find("btnLowest").GetComponent<Button>();
            btnHighHP = GameObject.Find("btnHighest").GetComponent<Button>();
            btnRandom = GameObject.Find("btnRandom").GetComponent<Button>();
            btnSell = GameObject.Find("btnSell").GetComponent<Button>();

            targetButtons = new Button[] { btnClosest, btnFurthest, btnLowHP, btnHighHP, btnRandom };

            txtSellValue = GameObject.Find("txtSellVal").GetComponent<TMP_Text>();

            btnClosest.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.Closest); });
            btnFurthest.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.Furthest); });
            btnLowHP.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.LowestHealth); });
            btnHighHP.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.HighestHealth); });
            btnRandom.onClick.AddListener(delegate { SetTargetMode(Tower.TargetMode.Random); });
            btnSell.onClick.AddListener(delegate { SellTower(); });

            pnlTowerPanel.SetActive(false);
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

            UpdateButtonColors(currentlySelectedTower.CurrentTargetMode);
        }

        private void UpdateButtonColors(Tower.TargetMode targetMode) {
            //  Remove highlight from each button
            for (int i = 0; i < targetButtons.Length; i++) {
                targetButtons[i].image.color = Color.white;
            }

            Button selectedBtn;

            switch (targetMode) {
                case Tower.TargetMode.Closest:
                    selectedBtn = btnClosest;
                    break;
                case Tower.TargetMode.Furthest:
                    selectedBtn = btnFurthest;
                    break;
                case Tower.TargetMode.Random:
                    selectedBtn = btnRandom;
                    break;
                case Tower.TargetMode.LowestHealth:
                    selectedBtn = btnLowHP;
                    break;
                case Tower.TargetMode.HighestHealth:
                    selectedBtn = btnHighHP;
                    break;
                default:
                    throw new System.Exception("Tower panel button does not exist for target mode: " + targetMode);
            }

            selectedBtn.image.color = selectedBtn.colors.selectedColor;
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
            UpdateButtonColors(targetMode);
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
