using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitRangeDetection{
    public void UnitEnteredRange(IUnit unit);
    public void UnitLeftRange(IUnit unit);

    public float GetRange();
}
