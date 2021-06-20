
public class Damage : IDamage {
    public IDamage.DamageType Type { get; private set; }

    public float Potency { get; private set; }

    public Status UnitStatus { get; private set; }

    public void Apply() {
        UnitStatus.TakeDamage(CalculateDamage());
    }

    public float CalculateDamage() {
        float resistence = UnitStatus.Stats[(int)Type];
        float effectiveDamage = (1 - resistence / 100) * Potency;
        return effectiveDamage;
    }
}