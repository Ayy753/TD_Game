public class StatMod : IStatMod{
    public float Potency { get; private set; }
    public Status.StatType Type { get; private set; }
    
    public void Apply(Unit unit) {
        unit.GetStatus().ModifyStat(Type, Potency);
    }
}