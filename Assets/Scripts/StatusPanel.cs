using System;
using System.Collections;
using System.Collections.Generic;
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

    private void UpdateStatusPanel() {
        txtName.text = targetUnit.GetName();
        txtCurrentHealth.text = Math.Round(targetStatus.Health.Value, 1).ToString();
        txtMaxHealth.text = Math.Round(targetStatus.Health.MaxHealth, 1).ToString();
        txtArmor.text = targetStatus.Armor.Value.ToString();
        txtFireResist.text = targetStatus.FireResist.Value.ToString();
        txtColdResist.text = targetStatus.ColdResist.Value.ToString();
        txtSpeed.text = targetStatus.Speed.Value.ToString();
        txtPoisonResist.text = targetStatus.PoisonResist.Value.ToString();
        txtLightningResist.text = targetStatus.LightningResist.Value.ToString();
        txtUnitDescription.text = targetUnit.GetDescription();
        healthBar.UpdateHealthBar();
    }

    public void TargetUnit(IUnit unit) {
        //  If a unit is already targetted, untarget it first
        if (targetStatus != null) {
            ClearTarget();
        }

        pnlStausPanel.SetActive(true);

        targetUnit = unit;
        targetStatus = unit.GetStatus();
        healthBar.Initialize(targetStatus);
        UpdateStatusPanel();

        targetStatus.OnStatusChanged += UpdateStatusPanel;
        targetStatus.OnStatusCleared += ClearTarget;
    }

    public void ClearTarget() {
        if (targetStatus != null) {
            targetStatus.OnStatusChanged -= UpdateStatusPanel;
            targetStatus.OnStatusCleared -= ClearTarget;
            targetStatus = null;
        }

        pnlStausPanel.SetActive(false);
    }
}