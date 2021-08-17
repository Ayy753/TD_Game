namespace DefaultNamespace.EffectSystem {
    using UnityEngine;

    public class DamageOverTime : IStatusEffect, IDamage {
        public float Duration { get; private set; }
        public float Potency { get; }
        public DamageType Type { get; private set; }
        public int RemainingTicks { get; private set; }

        private Status unitStatus;
        private float damagePerTick;

        public DamageOverTime(float potency, float duration, DamageType damageType) {
            Duration = duration;
            Potency = potency;
            Type = damageType;
        }

        public void Apply(Status status) {
            unitStatus = status;
            RemainingTicks = Mathf.CeilToInt(Duration / TickManager.tickFrequency);
            damagePerTick = CalculateEffectiveDamage(unitStatus);
        }

        public float CalculateEffectiveDamage(Status effectableStatus) {
            float resistence = unitStatus.GetStat((Status.StatType)Type).Value;
            float effectiveDamage = (1 - resistence / 100) * Potency;
            float damagerPerTick = effectiveDamage / (Duration * (1 / TickManager.tickFrequency));

            return damagerPerTick;
        }

        public void OnTick() {
            RemainingTicks -= 1;
            if (RemainingTicks > 0) {
                unitStatus.TakeDamage(damagePerTick);
            }
        }

        public void Remove() {
            //  Does nothing for now
        }

        public IStatusEffect Clone() {
            return (IStatusEffect)this.MemberwiseClone();
        }
    }
}
