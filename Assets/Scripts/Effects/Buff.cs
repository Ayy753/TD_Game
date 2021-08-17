namespace DefaultNamespace.EffectSystem {
    using UnityEngine;

    public class Buff : IStatusEffect, IStatMod {
        public float Duration { get; private set; }
        public float Potency { get; }
        public Status.StatType Type { get; private set; }
        public int RemainingTicks { get; private set; }

        private Status unitStatus;

        public Buff(float potency, float duration, Status.StatType statType) {
            Potency = potency;
            Type = statType;
            Duration = duration;
        }

        public void Apply(Status status) {
            unitStatus = status;
            unitStatus.ModifyStat(Type, Potency);
            RemainingTicks = Mathf.CeilToInt(Duration / TickManager.tickFrequency);
        }

        public void OnTick() {
            RemainingTicks -= 1;
        }

        public void Remove() {
            unitStatus.ModifyStat(Type, -Potency);
        }

        public IStatusEffect Clone() {
            return (IStatusEffect)this.MemberwiseClone();
        }
    }
}
