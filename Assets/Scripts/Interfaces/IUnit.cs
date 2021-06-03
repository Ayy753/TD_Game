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
}
