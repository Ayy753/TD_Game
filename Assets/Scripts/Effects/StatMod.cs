public class StatMod :  IStatMod {
    public float Potency { get; }
    public Status.StatType Type { get; private set; }

    public StatMod(float potency, Status.StatType statType) {
        Potency = potency;
        Type = statType;
    }

    public void Apply(Status status) {
        status.ModifyStat(Type, Potency);
    }
}