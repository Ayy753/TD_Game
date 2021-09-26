namespace DefaultNamespace.EffectSystem {

    using DefaultNamespace.StatusSystem;
    using UnityEngine;

    public class DamageOverTime : IStatusEffect, IDamage {
        public float Duration { get; private set; }
        public float Potency { get; }
        public DamageType Type { get; private set; }
        public int RemainingTicks { get; private set; }
        public float  DamagePerTick { get; set; }
        public bool Expires { get; private set; }

        public DamageOverTime(float potency, float duration, DamageType damageType, bool expires) {
            Duration = duration;
            Potency = potency;
            Type = damageType;
            Expires = expires;
        }

        public void Apply(Status status) {
            RemainingTicks = Mathf.CeilToInt(Duration / TickManager.tickFrequency);
            DamagePerTick = CalculateEffectiveDamage(status);
        }

        public float CalculateEffectiveDamage(Status effectableStatus) {
            float resistence = effectableStatus.GetStat((StatType)Type).Value;
            float effectiveDamage = (1 - resistence / 100) * Potency;
            float damagerPerTick = effectiveDamage / (Duration * (1 / TickManager.tickFrequency));

            return damagerPerTick;
        }

        public void OnTick() {
            if (Expires) {
                RemainingTicks -= 1;
            }
            //  Total effect tick damage is applied by Status 
        }

        public void Remove() {
            //  Does nothing 
        }

        public IStatusEffect Clone() {
            return (IStatusEffect)this.MemberwiseClone();
        }
    }
}
