public class DamageOverTime : IStatusEffect, IDamage{
    public float Duration { get; private set; }
    public float Potency { get; }
    public IDamage.DamageType Type { get; private set; }
    public float RemainingDuration { get; private set; }

    private Status unitStatus;

    //  Deals 1/nth the potency in damage every tick
    private static float damageRatio = 1 / 10;

    public DamageOverTime(float potency, float duration, IDamage.DamageType damageType) {
        Duration = duration;
        Potency = potency;
        Type = damageType;
    }

    public void Apply(Unit unit) {
        Status unitStatus = unit.GetStatus();
    }

    public float CalculateDamage(Status unitStatus) {
        float resistence = unitStatus.GetStat((Status.StatType)Type);
        float effectiveDamage = (1 - resistence / 100) * Potency * damageRatio;
        return effectiveDamage;
    }

    public void OnTick() {
        float effectiveDamage = CalculateDamage(unitStatus);
        unitStatus.TakeDamage(effectiveDamage);
        RemainingDuration -= TickManager.tickFrequency;
    }

    public void Remove() {
        //  Does nothing for now
    }
}