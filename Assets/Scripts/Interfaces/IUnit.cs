using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    /// <summary>
    /// Used by IUnitInput to alert IUnit it reached destination
    /// </summary>
    public void ReachedDestination();

    public void Died();

    public Status GetStatus();

    public void ApplyDamage(Damage.DamageTypeAndAmount[] damages);

    /// <summary>
    /// Get the transform component of the unit's gameobject
    /// </summary>
    /// <returns>Unit's gameobject transform component</returns>
    public Transform GetTransform();
}
