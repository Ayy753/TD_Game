using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitRangeDetection{
    public void UnitEnteredRange(Unit unit);
    public void UnitLeftRange(Unit unit);

    public float GetRange();
}
