using UnityEngine;

[CreateAssetMenu(fileName = "New StatMod", menuName = "Effect/StatMod")]
public class StatMod : ScriptableObject, IStatMod{
    [field: SerializeField] public float Potency { get; private set; }
    [field: SerializeField] public Status.StatType Type { get; private set; }
    
    public void Apply(Unit unit) {
        unit.GetStatus().ModifyStat(Type, Potency);
    }
}