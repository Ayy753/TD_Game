public class StatMod : IStatMod{
    public float Potency { get; private set; }
    public Status.StatType Type { get; private set; }

    public StatMod(float potency, Status.StatType statType) {
        Potency = potency;
        Type = statType;
    }

    public void Apply(Unit unit) {
        unit.GetStatus().ModifyStat(Type, Potency);
    }
}