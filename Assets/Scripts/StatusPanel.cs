using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class StatusPanel : IInitializable {
    TMP_Text txtCurrentHealth, txtMaxHealth, txtARmor, txtFireResist, txtColdResist, txtSpeed, txtPoisonResist, txtLightningResist;
    
    public void Initialize() {
        Debug.Log("initializing status panel");

        txtCurrentHealth = GameObject.Find("txtHealthCurrentVal").GetComponent<TMP_Text>();
        txtMaxHealth = GameObject.Find("txtHealthMaxVal").GetComponent<TMP_Text>();
        txtARmor = GameObject.Find("txtArmorVal").GetComponent<TMP_Text>();
        txtFireResist = GameObject.Find("txtFireResistVal").GetComponent<TMP_Text>();
        txtColdResist = GameObject.Find("txtColdResistVal").GetComponent<TMP_Text>();
        txtSpeed = GameObject.Find("txtSpeedVal").GetComponent<TMP_Text>();
        txtPoisonResist = GameObject.Find("txtPoisonResistVal").GetComponent<TMP_Text>();
        txtLightningResist = GameObject.Find("txtLightningResistVal").GetComponent<TMP_Text>();
    }

    public void UpdateStatusPanel(Status status) {
        txtCurrentHealth.text = status.Health.ToString();
        txtMaxHealth.text = status.MaxHealth.ToString();
        txtARmor.text = status.Armor.ToString();
        txtFireResist.text = status.FireResist.ToString();
        txtColdResist.text = status.ColdResist.ToString();
        txtSpeed.text = status.Speed.ToString();
        txtPoisonResist.text = status.PoisonResist.ToString();
        txtLightningResist.text = status.LightningResist.ToString();
    }
}
