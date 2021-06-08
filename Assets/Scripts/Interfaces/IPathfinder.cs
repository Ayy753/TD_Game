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
    /// Get path index at position
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Path index, or -1 if position isn't on path</returns>
    public int GetPathIndexAtPosition(Vector3Int position);

    /// <summary>
    /// Fires when path is recalculated
    /// </summary>
    public event EventHandler PathRecalculated;
}