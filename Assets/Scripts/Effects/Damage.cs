using UnityEngine;

public class Damage : IDamage {
    public float Potency { get; }
    public IDamage.DamageType Type { get; private set; }

    public Damage(float potency, IDamage.DamageType damageType) {
        Potency = potency;
        Type = damageType;
        Debug.Log("created damage object with potency of " + potency);
    }

    public void Apply(IUnit unit) {
        Status status = unit.GetStatus();
        float effectiveDamage = CalculateDamage(status);
        status.TakeDamage(effectiveDamage);

        Debug.Log("dealt damage: " + effectiveDamage);
    }

    public float CalculateDamage(Status unitStatus) {
        float resistence = unitStatus.GetStat((Status.StatType)Type);
        float effectiveDamage = (1 - resistence / 100) * Potency;
        return effectiveDamage;
    }
}