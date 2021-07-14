using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour{
    private Transform healthBarForeground;
    private Status status;

    private void Awake() {
        healthBarForeground = transform.Find("HealthbarFront");
    }

    public void OnEnable() {
        healthBarForeground.gameObject.transform.localScale = new Vector3(1, 0.25f, 1);
    }

    private void OnDisable() {
        if (status != null) {
            status.OnStatusChanged -= HandleStatChange;
            status = null;
        }
    }

    public void Initialize(Status status) {
        this.status = status;
        UpdateHealthBar();
        status.OnStatusChanged += HandleStatChange;
    }

    private void UpdateHealthBar() {
        Health health = status.Health;
        float healthPercent = health.Value / health.MaxHealth;
        if (healthPercent < 0) {
            healthPercent = 0;
        }
        healthBarForeground.gameObject.transform.localScale = new Vector3(healthPercent, 0.25f, 1);
    }

    private void HandleStatChange(Status.StatType statType) {
        if (statType == Status.StatType.Health) {
            UpdateHealthBar();
        }
    }
}
