using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder {
    /// <summary>
    /// Returns main path through the level
    /// </summary>
    /// <returns></returns>
    public List<Vector3Int> GetMainPath();

    /// <summary>
    /// Pathfinding to join main path
    /// </summary>
    /// <returns></returns>
    public List<Vector3Int> GetRouteToMainPath(Vector3Int currentPosition);

    /// <summary>
    /// Fires when path is recalculated
    /// </summary>
    public event EventHandler PathRecalculated;
}