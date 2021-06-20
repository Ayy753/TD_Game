using System;
using UnityEngine;

public abstract class Unit : MonoBehaviour, Itargetable
{
    public abstract event EventHandler TargetDisabled;

    /// <summary>
    /// Used by IUnitInput to alert IUnit it reached destination
    /// </summary>
    public abstract void ReachedDestination();

    public abstract void Died();

    public abstract Status GetStatus();

    //public abstract void ApplyDamage(Damage.DamageTypeAndAmount[] damages);

    /// <summary>
    /// Get the transform component of the unit's gameobject
    /// </summary>
    /// <returns>Unit's gameobject transform component</returns>
    public abstract Transform GetTransform();

    public abstract string GetName();
}
