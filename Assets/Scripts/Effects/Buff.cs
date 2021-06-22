using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Effect/Buff")]
public class Buff : ScriptableObject, IStatusEffect, IStatMod{
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float Potency { get; private set; }
    [field: SerializeField] public Status.StatType Type { get; private set; }
    public float RemainingDuration { get; private set; }


    private Status unitStatus;

    public void Apply(Unit unit) {
        unitStatus.ModifyStat(Type, Potency);
    }

    public void OnTick() {
        RemainingDuration -= TickManager.tickFrequency;
    }

    public void Remove() {
        unitStatus.ModifyStat(Type, -Potency);
    }
}