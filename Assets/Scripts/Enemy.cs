namespace DefaultNamespace {
    using DefaultNamespace.StatusSystem;
    using System;
    using UnityEngine;
    using Zenject;

    public class Enemy : MonoBehaviour, IUnit {
        [field: SerializeField] public EnemyData EnemyData { get; private set; }
        public int ModifiedValue { get; set; }

        public Status Status { get; set; }
        private HealthBar healthBar;

        public delegate void EnemyReachedGate(Enemy enemy);
        public delegate void EnemyDied(Enemy enemy);

        public static event EnemyReachedGate OnEnemyReachedGate;
        public static event EnemyDied OnEnemyDied;
        public event EventHandler TargetDisabled;
        public event EventHandler<UnitTookDamageEventArgs> OnUnitTookDamage;

        private bool hasAbility;
        private int ticksUntilNextEffect;
        private int ticksPerCooldown;

        private void Awake() {
            healthBar = transform.parent.GetComponentInChildren<HealthBar>();
            Status = new Status(EnemyData);

            if (EnemyData.EffectGroup != null) {
                hasAbility = true;
                ticksPerCooldown = (int)(EnemyData.EffectGroup.Cooldown / TickManager.tickFrequency);
                ticksUntilNextEffect = ticksPerCooldown;

                Debug.Log($"cooldown: {EnemyData.EffectGroup.Cooldown} tickfrequency: {TickManager.tickFrequency} ticks per cooldown: {ticksPerCooldown}");
            }
            else {
                hasAbility = false;
            }
        }

        private void OnEnable() {
            Status.Initialize();
            healthBar.Initialize(Status);

            Status.OnStatusChanged += Status_OnStatusChanged;
            TickManager.OnTick += TickManager_OnTick;
        }

        private void OnDisable() {
            Status.OnStatusChanged -= Status_OnStatusChanged;
            TickManager.OnTick -= TickManager_OnTick;
        }

        private void Status_OnStatusChanged(StatType statType, float amount) {
            if (statType == StatType.Health && amount < 0) {

                OnUnitTookDamage?.Invoke(this, new UnitTookDamageEventArgs(amount));

                if (Status.Health.Value <= 0) {
                    Died();
                }
            }
        }

        private void TickManager_OnTick() {
            if (hasAbility) {
                ticksUntilNextEffect--;
                if (ticksUntilNextEffect <= 0) {
                    EnemyData.EffectGroup.EffectArea(transform.position);
                    ticksUntilNextEffect = ticksPerCooldown;
                }
            }
        }

        public void ReachedDestination() {
            Despawn();
            if (OnEnemyReachedGate != null) {
                OnEnemyReachedGate.Invoke(this);
            }
        }

        private void Died() {
            if (OnEnemyDied != null) {
                OnEnemyDied.Invoke(this);
            }
            Despawn();
        }

        public void Despawn() {
            TargetDisabled?.Invoke(this, EventArgs.Empty);

            Status.Disabled();
            transform.parent.gameObject.SetActive(false);
        }

        public Transform GetTransform() {
            return transform;
        }

        public float GetValue() {
            return ModifiedValue;
        }

        public int GetEnemyId() {
            return EnemyData.EnemyId;
        }

        public string GetDescription() {
            return EnemyData.Description;
        }

        public string GetName() {
            return EnemyData.Name;
        }

        public CharacterData GetCharacterData() {
            return EnemyData;
        }

        public class Factory : PlaceholderFactory<int, Enemy> { }
    }
}
