using UnityEngine;

[CreateAssetMenu(fileName = "New Damage", menuName = "Effect/Damage")]
public class Damage : ScriptableObject, IDamage {
    [field: SerializeField] public float Potency { get; private set; }
    [field: SerializeField] public IDamage.DamageType Type { get; private set; }
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