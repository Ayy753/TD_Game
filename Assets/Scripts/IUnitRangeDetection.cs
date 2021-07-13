using UnityEngine;
using System.Collections.Generic;
public interface IUnitRangeDetection{
    public float Radius { get; }
    public List<IUnit> GetUnitsInRange(Vector3 center);
}
