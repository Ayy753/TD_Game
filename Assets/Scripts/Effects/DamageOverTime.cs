using UnityEngine;

public class DamageOverTime : MonoBehaviour, IStatusEffect, IDamage{
    public float Duration { get; private set; }
    public float Potency { get; private set; }
    public Status UnitStatus { get; private set; }
    public IDamage.DamageType Type { get; private set; }

    public void Apply() {
        throw new System.NotImplementedException();
    }

    public float CalculateDamage() {
        throw new System.NotImplementedException();
    }

    public void OnTick() {
        throw new System.NotImplementedException();
    }

    public void Remove() {
        throw new System.NotImplementedException();
    }
}