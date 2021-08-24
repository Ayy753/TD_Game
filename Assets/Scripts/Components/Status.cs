namespace DefaultNamespace.StatusSystem {
    using DefaultNamespace.EffectSystem;
    using System.Collections.Generic;

    public enum StatType {
        Armor, ColdResist, FireResist, PoisonResist, LightningResist, Health, Speed, Max
    }

    public class Status {
        private readonly Stat[] Stats;

        public Resistance Armor { get; private set; }
        public Resistance ColdResist { get; private set; }
        public Resistance FireResist { get; private set; }
        public Resistance PoisonResist { get; private set; }
        public Resistance LightningResist { get; private set; }
        public Health Health { get; private set; }
        public Speed Speed { get; private set; }
        public bool IsDead { get; private set; }

        private List<IStatusEffect> statusEffects;

        public delegate void StatusChanged(StatType statType, float amount);
        public event StatusChanged OnStatusChanged;

        public delegate void ClearStatus();
        public event ClearStatus OnStatusCleared;

        public Status(CharacterData characterData) {
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

        public void Initialize() {
            TickManager.OnTick += OnTick;

            statusEffects = new List<IStatusEffect>();
            for (int i = 0; i < Stats.Length; i++) {
                Stats[i].Initialize();
            }
            IsDead = false;
        }

        public void Disabled() {
            TickManager.OnTick -= OnTick;
            OnStatusCleared?.Invoke();
        }

        public void TakeDamage(float effectiveDamage) {
            if (!IsDead) {
                Health.TakeDamage(effectiveDamage);

                if (Health.Value <= 0) {
                    IsDead = true;
                }

                OnStatusChanged?.Invoke(StatType.Health, -effectiveDamage);
            }
        }

        public void RestoreHealth(float amount) {
            Health.Heal(amount);
            OnStatusChanged?.Invoke(StatType.Health, amount);
        }

        public void ModifyStat(StatType type, float amount) {
            Stats[(int)type].ModifyStat(amount);
            OnStatusChanged?.Invoke(type, amount);
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

        public Stat GetStat(StatType type) {
            return Stats[(int)type];
        }
    }
}
