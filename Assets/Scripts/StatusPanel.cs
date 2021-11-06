namespace DefaultNamespace.GUI {

    using DefaultNamespace;
    using DefaultNamespace.StatusSystem;
    using System;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class StatusPanel : IInitializable {
        GameObject pnlStausPanel;
        TMP_Text txtName, txtCurrentHealth, txtMaxHealth, txtArmor, txtFireResist, txtColdResist, txtSpeed, txtPoisonResist, txtLightningResist, txtUnitDescription;
        HealthBar healthBar;

        private Status targetStatus;
        private IUnit targetUnit;

        public void Initialize() {
            Debug.Log("initializing status panel");

            pnlStausPanel = GameObject.Find("pnlStatus");

            txtName = GameObject.Find("txtStatusName").GetComponent<TMP_Text>();
            txtCurrentHealth = GameObject.Find("txtHealthCurrentVal").GetComponent<TMP_Text>();
            txtMaxHealth = GameObject.Find("txtHealthMaxVal").GetComponent<TMP_Text>();
            txtArmor = GameObject.Find("txtArmorVal").GetComponent<TMP_Text>();
            txtFireResist = GameObject.Find("txtFireResistVal").GetComponent<TMP_Text>();
            txtColdResist = GameObject.Find("txtColdResistVal").GetComponent<TMP_Text>();
            txtSpeed = GameObject.Find("txtSpeedVal").GetComponent<TMP_Text>();
            txtPoisonResist = GameObject.Find("txtPoisonResistVal").GetComponent<TMP_Text>();
            txtLightningResist = GameObject.Find("txtLightningResistVal").GetComponent<TMP_Text>();
            txtUnitDescription = GameObject.Find("txtUnitDescription").GetComponent<TMP_Text>();

            healthBar = GameObject.Find("pnlStatus").GetComponentInChildren<HealthBar>();

            pnlStausPanel.SetActive(false);
        }

        private void PopulateStatusPanel() {
            txtName.text = targetUnit.GetName();
            txtCurrentHealth.text = Math.Round(targetStatus.Health.Value, 1).ToString();
            txtMaxHealth.text = Math.Round(targetStatus.Health.MaxHealth, 1).ToString();
            txtArmor.text = Math.Round(targetStatus.Armor.Value, 1).ToString();
            txtFireResist.text = Math.Round(targetStatus.FireResist.Value, 1).ToString();
            txtColdResist.text = Math.Round(targetStatus.ColdResist.Value, 1).ToString();
            txtSpeed.text = Math.Round(targetStatus.Speed.Value, 1).ToString();
            txtPoisonResist.text = Math.Round(targetStatus.PoisonResist.Value, 1).ToString();
            txtLightningResist.text = Math.Round(targetStatus.LightningResist.Value, 1).ToString();
            txtUnitDescription.text = targetUnit.GetDescription();
        }

        private void TargetStatus_OnStatusChanged(StatType statType, float amount) {
            UpdateStatusPanel(statType);
        }

        private void TargetStatus_OnStatusCleared() {
            ClearTarget();
        }

        private void UpdateStatusPanel(StatType statType) {
            string statValueRounded = Math.Round(targetStatus.GetStat(statType).Value, 1).ToString();

            Color textColor = StatModColor(statType);

            switch (statType) {
                case StatType.Armor:
                    txtArmor.text = statValueRounded;
                    txtArmor.color = textColor;
                    break;
                case StatType.ColdResist:
                    txtColdResist.text = statValueRounded;
                    txtColdResist.color = textColor;
                    break;
                case StatType.FireResist:
                    txtFireResist.text = statValueRounded;
                    txtFireResist.color = textColor;
                    break;
                case StatType.PoisonResist:
                    txtPoisonResist.text = statValueRounded;
                    txtPoisonResist.color = textColor;
                    break;
                case StatType.LightningResist:
                    txtLightningResist.text = statValueRounded;
                    txtLightningResist.color = textColor;
                    break;
                case StatType.Health:
                    txtCurrentHealth.text = statValueRounded;
                    txtMaxHealth.text = Math.Round(targetStatus.Health.MaxHealth, 1).ToString();
                    break;
                case StatType.Speed:
                    txtSpeed.text = statValueRounded;
                    txtSpeed.color = textColor;
                    break;
            }
        }

        private Color StatModColor(StatType statType) {
            Stat SelectedStat = targetStatus.GetStat(statType);
            if (SelectedStat.Value == SelectedStat.BaseValue) {
                return Color.white;
            }
            if (SelectedStat.Value > SelectedStat.BaseValue) {
                return Color.green;
            }
            else {
                return Color.red;
            }
        }

        public void TargetUnit(IUnit unit) {
            //  If a unit is already targetted, untarget it first
            if (targetStatus != null) {
                ClearTarget();
            }

            pnlStausPanel.SetActive(true);

            targetUnit = unit;
            targetStatus = unit.Status;
            healthBar.Initialize(targetStatus);
            PopulateStatusPanel();

            targetStatus.OnStatusChanged += TargetStatus_OnStatusChanged;
            targetStatus.OnStatusCleared += TargetStatus_OnStatusCleared;
        }

        public void ClearTarget() {
            if (targetStatus != null) {
                targetStatus.OnStatusChanged -= TargetStatus_OnStatusChanged;
                targetStatus.OnStatusCleared -= TargetStatus_OnStatusCleared;
            }

            pnlStausPanel.SetActive(false);
        }
    }
}
