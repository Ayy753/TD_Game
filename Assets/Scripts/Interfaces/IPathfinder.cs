namespace DefaultNamespace {

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
        /// A preview of the path if this position was blocked
        /// </summary>
        /// <returns></returns>
        public List<Vector3Int> GetBuildPreviewPath(Vector3Int positionBlocked);

        /// <summary>
        /// A preview of the path if this position was demolished
        /// </summary>
        /// <returns></returns>
        public List<Vector3Int> GetDemolishPreviewPath(Vector3Int positionDemolished);

        /// <summary>
        /// Pathfinding to join main path
        /// </summary>
        /// <returns></returns>
        public (List<Vector3Int>, int) GetRouteToMainPath(Vector3Int currentPosition);

        /// <summary>
        /// Get path index at position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Path index, or -1 if position isn't on path</returns>
        public int GetPathIndexAtPosition(Vector3Int position);

        public bool IsOnMainPath(Vector3Int position);

        /// <summary>
        /// Fires when path is recalculated
        /// </summary>
        public event EventHandler PathRecalculated;
    }
}
