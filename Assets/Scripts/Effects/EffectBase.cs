using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class exists soley to compensate for Unity's inability to serialize objects based on interfaces
/// </summary>
public abstract class EffectBase{
    /// <summary>
    /// The strength of the effect
    /// </summary>
    public abstract float Potency { get; }

    /// <summary>
    /// Apply the effect to Unit
    /// </summary>
    public abstract void Apply(Unit unit);
}