using UnityEngine;
using System.Collections.Generic;
public interface IEffectableRangeDetection{
    public float Radius { get; }
    public List<IEffectable> GetEffectableObjectsInRange(Vector3 center);
}
