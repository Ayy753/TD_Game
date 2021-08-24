namespace DefaultNamespace.StatusSystem {
    using DefaultNamespace.EffectSystem;
    using System.Collections.Generic;
    using UnityEngine;

    public enum StatType {
        Armor, ColdResist, FireResist, PoisonResist, LightningResist, Health, Speed, Max
    }

    public class Status : MonoBehaviour {
        [field: SerializeField] public CharacterData characterData { get; private set; }
        private IUnit unit;

        private Stat[] Stats;

        public Resistance Armor { get; private set; }
        public Resistance ColdResist { get; private set; }
        public Resistance FireResist { get; private set; }
        public Resistance PoisonResist { get; private set; }
        public Resistance LightningResist { get; private set; }
        public Health Health { get; private set; }
        public Speed Speed { get; private set; }
        public bool IsDead { get; private set; }

        private List<IStatusEffect> statusEffects;

        public delegate void StatusChanged(StatType statType);
        public delegate void ClearStatus();
        
        public event StatusChanged OnStatusChanged;


        private void Awake() {
            unit = transform.GetComponent<IUnit>();

            Armor = new Resistance(characterData.BaseArmor);
            ColdResist = new Resistance(characterData.BaseColdResist);
            FireResist = new Resistance(characterData.BaseFireResist);
            PoisonResist = new Resistance(characterData.BasePoisonResist);
            LightningResist = new Resistance(characterData.BaseLightningResist);
            Health = new Health(characterData.BaseHealth);
            Speed = new Speed(characterData.BaseSpeed);

            Stats = new Stat[] {
                Armor, ColdResist, FireResist, PoisonResist, LightningResist, Health, Speed
            };
        }

        private void OnEnable() {
            TickManager.OnTick += OnTick;

            statusEffects = new List<IStatusEffect>();
            for (int i = 0; i < Stats.Length; i++) {
                Stats[i].Initialize();
            }
            IsDead = false;
        }

        private void OnDisable() {
            TickManager.OnTick -= OnTick;
        }

        public void TakeDamage(float effectiveDamage) {
            if (!IsDead) {
                Health.TakeDamage(effectiveDamage);

                if (Health.Value <= 0) {
                    IsDead = true;
                    unit.Died();
                }

                if (OnStatusChanged != null) {
                    OnStatusChanged.Invoke(StatType.Health);
                }
            }
        }

        public void RestoreHealth(float amount) {
            Health.Heal(amount);

            if (OnStatusChanged != null) {
                OnStatusChanged.Invoke(StatType.Health);
            }
        }

        public void ModifyStat(StatType type, float amount) {
            Stats[(int)type].ModifyStat(amount);

            if (OnStatusChanged != null) {
                OnStatusChanged.Invoke(type);
            }
        }

        public void ApplyEffectGroup(EffectGroup effectGroup) {
            foreach (IEffect effect in effectGroup.GetEffects()) {
                if (effect is IStatusEffect statusEffect) {
                    statusEffects.Add(statusEffect);
                }
                if (!(effect is Damage)) {
                    effect.Apply(this);
                }
            }

            ApplyAllInstantDamage(effectGroup);
        }

        private void ApplyAllInstantDamage(EffectGroup effectGroup) {
            float totalDamage = 0;

            foreach (IEffect effect in effectGroup.GetEffects()) {
                if (effect is Damage damage) {
                    totalDamage += damage.CalculateEffectiveDamage(this);
                }
            }

            if (totalDamage > 0) {
                TakeDamage(totalDamage);
            }
        }

        private void OnTick() {
            float totalDamageThisTick = 0;
            IStatusEffect effect;

            for (int i = 0; i < statusEffects.Count; i++) {
                effect = statusEffects[i];

                if (effect is DamageOverTime damageOverTime) {
                    totalDamageThisTick += damageOverTime.DamagePerTick;
                }
                effect.OnTick();
                
                if (effect.RemainingTicks <= 0) {
                    effect.Remove();
                    statusEffects.Remove(effect);
                    i--;
                }
            }

            if (totalDamageThisTick > 0) {
                TakeDamage(totalDamageThisTick);
            }
        }

        public CharacterData GetCharacterData() {
            return characterData;
        }

        public Stat GetStat(StatType type) {
            return Stats[(int)type];
        }
    }
}
