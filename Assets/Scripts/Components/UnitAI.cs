namespace DefaultNamespace {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    /// <summary>
    /// Moves through the path
    /// </summary>
    public class UnitAI : MonoBehaviour, IUnitInput {
        IPathfinder pathFinder;
        IUnit unit;

        private List<Vector3Int> mainPath;
        private int pathIndex;

        private List<Vector3Int> routeToMainPath;
        private int routeIndex;

        private bool onMainPath;

        private Vector3Int nextTilePosition;

        private void Awake() {
            pathFinder = GameObject.Find("PathFinder").GetComponent<IPathfinder>();
            unit = transform.GetComponent<IUnit>();
        }

        private void OnEnable() {
            mainPath = pathFinder.GetMainPath();

            if (mainPath.Count == 0)
                throw new System.Exception("There is no path");

            pathIndex = 0;
            nextTilePosition = mainPath[pathIndex];

            onMainPath = true;

            pathFinder.PathRecalculated += OnPathRecalculated;
            unit.TargetDisabled += UnitAI_TargetDisabled;
        }

        private void OnDisable() {
            unit.TargetDisabled -= UnitAI_TargetDisabled;
        }

        private void UnitAI_TargetDisabled(object sender, EventArgs e) {
            transform.parent.position = mainPath[0];
        }

        private void OnPathRecalculated(object sender, EventArgs e) {
            Vector3Int currentPosition = Vector3Int.FloorToInt(unit.GetTransform().position);

            if (pathFinder.IsOnMainPath(currentPosition) == false) {
                (List<Vector3Int>, int) item = pathFinder.GetRouteToMainPath(currentPosition);

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
                    pathFinder.PathRecalculated -= OnPathRecalculated;
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
}
