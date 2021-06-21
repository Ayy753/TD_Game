using UnityEngine;

public class StatMod : MonoBehaviour, IStatMod{
    public Status.StatType Type { get; private set; }

    public float Potency { get; private set; }

    public Status UnitStatus { get; private set; }

    public void Apply() {
        throw new System.NotImplementedException();
    }
}