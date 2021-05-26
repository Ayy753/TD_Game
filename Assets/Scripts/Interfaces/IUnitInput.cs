using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitInput
{
    public void ReachedNextTile();
    public Vector3Int GetNextTile();
    public void Initialize();
}
