public class Buff : IStatusEffect, IStatMod{
    public float Duration { get; private set; }
    public float Potency { get; }
    public Status.StatType Type { get; private set; }
    public float RemainingDuration { get; private set; }
    
    private Status unitStatus;

    public Buff(float potency, float duration, Status.StatType statType) {
        Potency = potency;
        Type = statType;
    }

    public void Apply(Unit unit) {
        Status unitStatus = unit.GetStatus();
        unitStatus.ModifyStat(Type, Potency);
    }

    public void OnTick() {
        RemainingDuration -= TickManager.tickFrequency;
    }

    public void Remove() {
        unitStatus.ModifyStat(Type, -Potency);
    }
}