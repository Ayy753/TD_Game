using UnityEngine;

public class DamageOverTime : MonoBehaviour, IStatusEffect, IDamage{
    public float Duration { get; private set; }
    public float Potency { get; private set; }
    public IDamage.DamageType Type { get; private set; }

    private Status unitStatus;

    //  Deals 1/nth the potency in damage every tick
    private static float damageRatio = 1 / 10;

    public void Apply(Unit unit) {
        unit.GetStatus().AddStatusEffect(this);
    }

    public float CalculateDamage(Status unitStatus) {
        float resistence = unitStatus.effectiveStats[(int)Type];
        float effectiveDamage = (1 - resistence / 100) * Potency * damageRatio;
        return effectiveDamage;
    }

    public void OnTick() {
        float effectiveDamage = CalculateDamage(unitStatus);
        unitStatus.TakeDamage(effectiveDamage);
    }

    public void Remove() {
        throw new System.NotImplementedException();
    }
}