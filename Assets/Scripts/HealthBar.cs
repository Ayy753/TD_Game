namespace DefaultNamespace {

    using DefaultNamespace.StatusSystem;
    using UnityEngine;

    public class HealthBar : MonoBehaviour {
        private Transform healthBarForeground;
        private Status status;

        private void Awake() {
            healthBarForeground = transform.Find("HealthbarFront");
        }

        public void OnEnable() {
            healthBarForeground.gameObject.transform.localScale = new Vector3(1, 0.25f, 1);
        }

        public void OnDisable() {
            if (status != null) {
                status.OnStatusChanged -= HandleStatChange;
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

        private void HandleStatChange(StatType statType, float amount) {
            if (statType == StatType.Health) {
                UpdateHealthBar();
            }
        }
    }
}
