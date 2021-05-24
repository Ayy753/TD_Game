using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Moves through the path
/// </summary>
public class UnitAI : IUnitInput
{

    private List<Vector3Int> mainPath;
    private int pathIndex;
    private Vector3Int nextTilePosition;

    //  TODO find own reference to pathfinder
    //  injection apparently doesn't work at runtime so I have to pass in pathfinder for now
    public UnitAI(IPathfinder pathfinder) {
        mainPath = pathfinder.GetMainPath();

        if (mainPath.Count == 0)
            throw new System.Exception("There is no path");

        pathIndex = 0;
        nextTilePosition = mainPath[pathIndex];
    }

    public void ReachedNextTile() {
        pathIndex++;
        if (pathIndex < mainPath.Count) {
            nextTilePosition = mainPath[pathIndex];
        }
    }

    public Vector3Int GetNextTile() {
        return nextTilePosition;
    }
}


