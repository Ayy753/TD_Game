public class Damage : IDamage {
    public float Potency { get; private set; }
    public IDamage.DamageType Type { get; private set; }
    public void Apply(Unit unit) {
        Status status = unit.GetStatus();
        float effectiveDamage = CalculateDamage(status);
        status.TakeDamage(effectiveDamage);
    }

    public float CalculateDamage(Status unitStatus) {
        float resistence = unitStatus.effectiveStats[(int)Type];
        float effectiveDamage = (1 - resistence / 100) * Potency;
        return effectiveDamage;
    }
}