using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour{
    private Transform healthBarForeground;
    private float maxHP;
    private Status status;

    private void Awake() {
        healthBarForeground = transform.Find("HealthbarFront");
    }

    public void Initialize(Status status, float maxHP) {
        this.status = status;
        this.maxHP = maxHP;
        healthBarForeground.gameObject.transform.localScale = new Vector3(1, 0.25f, 1);
    }

    public void UpdateHealthBar() {
        float healthPercent = status.Health / maxHP;
        if (healthPercent < 0) {
            healthPercent = 0;
        }
        healthBarForeground.gameObject.transform.localScale = new Vector3(healthPercent, 0.25f, 1);
    }
}
