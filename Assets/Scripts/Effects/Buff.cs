namespace DefaultNamespace.EffectSystem {
    using DefaultNamespace.StatusSystem;
    using UnityEngine;

    public class Buff : IStatusEffect, IStatMod {
        public float Duration { get; private set; }
        public float Potency { get; }
        public StatType Type { get; private set; }
        public int RemainingTicks { get; private set; }
        public bool Expires { get; private set; }

        private Status unitStatus;

        public Buff(float potency, float duration, StatType statType, bool expires) {
            Potency = potency;
            Type = statType;
            Duration = duration;
            Expires = expires;
        }

        public void Apply(Status status) {
            unitStatus = status;
            unitStatus.ModifyStat(Type, Potency);
            RemainingTicks = Mathf.CeilToInt(Duration / TickManager.tickFrequency);
        }

        public void OnTick() {
            if (Expires) {
                RemainingTicks -= 1;
            }
        }

        public void Remove() {
            unitStatus.ModifyStat(Type, -Potency);
        }

        public IStatusEffect Clone() {
            return (IStatusEffect)this.MemberwiseClone();
        }
    }
}
