using UnityEngine;

public class Damage : IDamage {
    public float Potency { get; }
    public IDamage.DamageType Type { get; private set; }

    public Damage(float potency, IDamage.DamageType damageType) {
        Potency = potency;
        Type = damageType;
        Debug.Log("created damage object with potency of " + potency);
    }

    public void Apply(Status status) {
        float effectiveDamage = CalculateDamage(status);
        status.TakeDamage(effectiveDamage);
    }

    public float CalculateDamage(Status unitStatus) {
        float resistence = unitStatus.GetStat((Status.StatType)Type).Value;
        float effectiveDamage = (1 - resistence / 100) * Potency;
        return effectiveDamage;
    }
}