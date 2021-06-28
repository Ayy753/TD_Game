using System;
using UnityEngine;

public interface IUnit : Itargetable {
    /// <summary>
    /// Used by IUnitInput to alert IUnit it reached destination
    /// </summary>
    public void ReachedDestination();

    public void Died();

    public Status GetStatus();

    public void ApplyDamage(Damage.DamageTypeAndAmount[] damages);
}
