using UnityEngine;

public class StatMod : MonoBehaviour, IStatMod{
    public Status.StatType Type { get; private set; }
    public float Potency { get; private set; }

    public void Apply(Unit unit) {
        unit.GetStatus().ModifyStat(Type, Potency);
    }
}