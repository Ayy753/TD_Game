using UnityEngine;

public class Debuff : IStatusEffect, IStatMod {
    public float Duration { get; private set; }
    public float Potency { get; private set; }
    public Status.StatType Type { get; private set; }
    public int RemainingTicks { get; private set; }
    public IDamage.DamageType ResistType { get; private set; }
    private Status unitStatus;

    public Debuff(float potency, float duration, Status.StatType statType, IDamage.DamageType resistType) {
        Potency = potency;
        Type = statType;
        Duration = duration;
        ResistType = resistType;
    }

    public void Apply(Status status) {
        unitStatus = status;
        
        float resistence = unitStatus.GetStat((Status.StatType)ResistType).Value;
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
        RemainingTicks -= 1;
    }

    public void Remove() {
        unitStatus.ModifyStat(Type, Potency);
    }

    public IStatusEffect Clone() {
        return (IStatusEffect)this.MemberwiseClone();
    }
}
