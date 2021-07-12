public class DamageOverTime : IStatusEffect, IDamage{
    public float Duration { get; private set; }
    public float Potency { get; }
    public IDamage.DamageType Type { get; private set; }
    public float RemainingDuration { get; private set; }

    private Status unitStatus;
    private float damagePerTick;

    public DamageOverTime(float potency, float duration, IDamage.DamageType damageType) {
        Duration = duration;
        Potency = potency;
        Type = damageType;
    }

    public void Apply(Status status) {
        unitStatus = status;
        RemainingDuration = Duration;
        damagePerTick = CalculateDamage(unitStatus);
    }

    public float CalculateDamage(Status unitStatus) {
        //  Get resistence value based on damage type
        float resistence = unitStatus.GetStat((Status.StatType)Type).Value;
        float effectiveDamage = (1 - resistence / 100) * Potency;
        float damagerPerTick = effectiveDamage / (Duration * (1 / TickManager.tickFrequency));

        return damagerPerTick;
    }

    public void OnTick() {
        unitStatus.TakeDamage(damagePerTick);
        RemainingDuration -= TickManager.tickFrequency;
    }

    public void Remove() {
        //  Does nothing for now
    }

    public IStatusEffect Clone() {
        return (IStatusEffect)this.MemberwiseClone();
    }
}