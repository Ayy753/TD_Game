using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Moves through the path
/// </summary>
public class UnitAI : IUnitInput
{
    IPathfinder pathFinder;
    IUnit unit;


    private List<Vector3Int> mainPath;
    private int pathIndex;

    private List<Vector3Int> routeToMainPath;
    private int routeIndex;

    private bool onMainPath;

    private Vector3Int nextTilePosition;

    //  TODO find own reference to pathfinder
    //  injection apparently doesn't work at runtime so I have to pass in pathfinder for now
    public UnitAI(IUnit unit, IPathfinder pathFinder) {
        this.unit = unit;
        this.pathFinder = pathFinder;
    }

    public void Initialize() {
        mainPath = pathFinder.GetMainPath();

        if (mainPath.Count == 0)
            throw new System.Exception("There is no path");

        pathIndex = 0;
        nextTilePosition = mainPath[pathIndex];

        onMainPath = true;

        pathFinder.PathRecalculated += OnPathRecalculated;
    }

    private void OnPathRecalculated(object sender, EventArgs e) {
        Vector3Int currentPosition = mainPath[pathIndex];

        if (pathFinder.IsOnMainPath(currentPosition) == false) {
            (List<Vector3Int>, int) item =  pathFinder.GetRouteToMainPath(currentPosition);

            routeToMainPath = item.Item1;

            //  Index where unit will end up at the end of route to main path
            pathIndex = item.Item2;

            routeIndex = 0;

            onMainPath = false;
        }
        else {
            pathIndex = pathFinder.GetPathIndexAtPosition(currentPosition);
            onMainPath = true;
        }

        mainPath = pathFinder.GetMainPath();
    }

    public void ReachedNextTile() {

        if (onMainPath) {
            pathIndex++;
            if (pathIndex < mainPath.Count) {
                nextTilePosition = mainPath[pathIndex];
            }
            else {
                unit.ReachedDestination();
            }
        }
        else {
            routeIndex++;
            if (routeIndex < routeToMainPath.Count) {
                nextTilePosition = routeToMainPath[routeIndex];
            }
            else {
                onMainPath = true;
            }
        }

    }

    public Vector3Int GetNextTile() {
        return nextTilePosition;
    }


}


