namespace DefaultNamespace.EffectSystem {
    using DefaultNamespace.StatusSystem;
    using UnityEngine;

    public class Debuff : IStatusEffect, IStatMod {
        public float Duration { get; private set; }
        public float Potency { get; private set; }
        public StatType Type { get; private set; }
        public int RemainingTicks { get; private set; }
        public DamageType ResistType { get; private set; }
        public bool Expires { get; private set; }

        private Status unitStatus;

        public Debuff(float potency, float duration, StatType statType, DamageType resistType, bool expires) {
            Potency = potency;
            Type = statType;
            Duration = duration;
            ResistType = resistType;
            Expires = expires;
        }

        public void Apply(Status status) {
            unitStatus = status;

            float resistence = unitStatus.GetStat((StatType)ResistType).Value;
            float effectiveness = (1 - resistence / 100);

            if (effectiveness < 0) {
                effectiveness = 0;
            }

            Potency = effectiveness * Potency;
            Duration = effectiveness * Duration;

            unitStatus.ModifyStat(Type, -Potency);
            RemainingTicks = Mathf.CeilToInt(Duration / TickManager.tickFrequency);
        }

        public void OnTick() {
            if (Expires) {
                RemainingTicks -= 1;
            }
        }

        public void Remove() {
            unitStatus.ModifyStat(Type, Potency);
        }

        public IStatusEffect Clone() {
            return (IStatusEffect)this.MemberwiseClone();
        }
    }
}
