using UnityEngine;

public class Buff : MonoBehaviour, IStatusEffect, IStatMod{
    public float Duration { get; private set; }
    public float Potency { get; private set; }
    public Status.StatType Type { get; private set; }

    public void Apply(Unit unit) {
        unit.GetStatus().AddStatusEffect(this);
    }

    public void OnTick() {
        throw new System.NotImplementedException();
    }

    public void Remove() {
        throw new System.NotImplementedException();
    }
}