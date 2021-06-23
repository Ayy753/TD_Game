public class Buff : IStatusEffect, IStatMod{
    public float Duration { get; private set; }
    public float Potency { get; private set; }
    public Status.StatType Type { get; private set; }
    public float RemainingDuration { get; private set; }
    
    private Status unitStatus;

    public void Apply(Unit unit) {
        unitStatus.ModifyStat(Type, Potency);
    }

    public void OnTick() {
        RemainingDuration -= TickManager.tickFrequency;
    }

    public void Remove() {
        unitStatus.ModifyStat(Type, -Potency);
    }
}