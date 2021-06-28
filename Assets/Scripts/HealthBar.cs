using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour{
    private Transform healthBarForeground;
    private Status status;

    private void Awake() {
        healthBarForeground = transform.Find("HealthbarFront");
        status = transform.parent.GetComponent<Status>();
    }

    public void Initialize(Status status) {
        this.status = status;
        healthBarForeground.gameObject.transform.localScale = new Vector3(1, 0.25f, 1);
    }

    public void UpdateHealthBar() {
        float healthPercent = status.CurrentHealth / status.MaxHealth;
        if (healthPercent < 0) {
            healthPercent = 0;
        }
        healthBarForeground.gameObject.transform.localScale = new Vector3(healthPercent, 0.25f, 1);
    }
}
