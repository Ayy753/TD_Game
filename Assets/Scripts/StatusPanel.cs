using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusPanel{
    TMP_Text txtCurrentHealth, txtMaxHealth, txtARmor, txtFireResist, txtColdResist, txtSpeed, txtPoisonResist, txtLightningResist;

    public void Awake() {
        txtCurrentHealth = GameObject.Find("txtHealthCurrentVal").GetComponent<TextMeshPro>();
        txtMaxHealth = GameObject.Find("txtHealthMaxVal").GetComponent<TextMeshPro>();
        txtARmor = GameObject.Find("txtArmorVal").GetComponent<TextMeshPro>();
        txtFireResist = GameObject.Find("txtFireResistVal").GetComponent<TextMeshPro>();
        txtColdResist = GameObject.Find("txtColdResistVal").GetComponent<TextMeshPro>();
        txtSpeed = GameObject.Find("txtSpeedVal").GetComponent<TextMeshPro>();
        txtPoisonResist = GameObject.Find("txtPoisonResistVal").GetComponent<TextMeshPro>();
        txtLightningResist = GameObject.Find("txtLightningResistVal").GetComponent<TextMeshPro>();
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
