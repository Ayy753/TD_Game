using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Over Time", menuName = "Effect/DamageOverTime")]
public class DamageOverTime : ScriptableObject, IStatusEffect, IDamage{
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float Potency { get; private set; }
    [field: SerializeField] public IDamage.DamageType Type { get; private set; }
    public float RemainingDuration { get; private set; }

    private Status unitStatus;

    //  Deals 1/nth the potency in damage every tick
    private static float damageRatio = 1 / 10;

    public void Apply(Unit unit) {
        //  Does nothing for now
    }

    public float CalculateDamage(Status unitStatus) {
        float resistence = unitStatus.effectiveStats[(int)Type];
        float effectiveDamage = (1 - resistence / 100) * Potency * damageRatio;
        return effectiveDamage;
    }

    public void OnTick() {
        float effectiveDamage = CalculateDamage(unitStatus);
        unitStatus.TakeDamage(effectiveDamage);
        RemainingDuration -= TickManager.tickFrequency;
    }

    public void Remove() {
        //  Does nothing for now
    }
}