namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class PathFinder : MonoBehaviour, IPathfinder, IInitializable, IDisposable {
        private List<Vector3Int> currentPath;
        private Transform entrance, exit;

        [Inject] private IMapManager mapManager;
        private PathCalculator pathCalculator;

        public event EventHandler PathRecalculated;

        public void Initialize() {
            Debug.Log("Initializing pathfinder");

            entrance = GameObject.Find("Entrance").transform;
            exit = GameObject.Find("Exit").transform;

            pathCalculator = new PathCalculator(mapManager, Vector3Int.FloorToInt(entrance.position), Vector3Int.FloorToInt(exit.position));

            if (entrance == null || exit == null) {
                Debug.LogError("entrance or exit not found");
            }
            else {
                CalculateMainPath();
            }

            BuildManager.OnStructureChanged += HandleStructureChanged;
        }

        public void Dispose() {
            BuildManager.OnStructureChanged -= HandleStructureChanged;
        }

        private void HandleStructureChanged(object sender, StructureChangedEventArgs e) {
            if (e.ChangeType == BuildMode.Build) {
                CalculateMainPath();
            }
            else if (e.ChangeType == BuildMode.Demolish) {
                CalculateMainPath();
            }
        }

        /// <summary>
        /// Used to estimate the heuristic value of a tile
        /// </summary>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        private int ManhattanDistance(Vector3Int start, Vector3Int finish) {
            return Mathf.Abs(finish.x - start.x) + Mathf.Abs(finish.y - start.y);
        }

        /// <summary>
        /// A tile is valid(walkable) if it doesn't contain a structure, but contains a ground tile
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsValidTile(Vector3Int position) {
            if (!mapManager.ContainsTileAt(MapLayer.StructureLayer, position) &&
                mapManager.ContainsTileAt(MapLayer.GroundLayer, position)) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Estimates the shortest path through the level using an implementation of the A* algorithm
        /// </summary>
        /// <returns>A chain of path nodes, or null if there is no valid path</returns>
        private void CalculateMainPath() {
            currentPath = pathCalculator.CalculatePath();
            if (currentPath != null) {
                PathRecalculated?.Invoke(this, EventArgs.Empty);
            }
        }

        public List<Vector3Int> GetBuildPreviewPath(Vector3Int positionBlocked) {
            return pathCalculator.GetBuildPreviewPath(positionBlocked);
        }

        public List<Vector3Int> GetDemolishPreviewPath(Vector3Int positionDemolished) {
            return pathCalculator.GetDemolishPreviewPath(positionDemolished);
        }

        public List<Vector3Int> GetPlatformBuiltPreviewPath(Vector3Int positionBuilt, float platformMovementCost) {
            return pathCalculator.GetPlatformBuiltPreviewPath(positionBuilt, platformMovementCost);
        }

        public List<Vector3Int> GetMainPath() {
            return currentPath;
        }

        public (List<Vector3Int>, int) GetRouteToMainPath(Vector3Int currentPosition) {
            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            PathNode initialNode = new PathNode(currentPosition);
            openList.Add(initialNode);

            while (openList.Count > 0) {
                PathNode shortest = openList[0];

                foreach (PathNode node in openList) {
                    if (node.Fcost < shortest.Fcost) {
                        shortest = node;
                    }
                }

                openList.Remove(shortest);
                closedList.Add(shortest);

                for (int i = 0; i < currentPath.Count; i++) {
                    //  If it is we found the shortest route to the main path
                    if (currentPath[i] == shortest.Coordinate) {
                        List<Vector3Int> path = shortest.GetPath();

                        //mapManager.HighlightPath(path, Color.grey);
                        return (path, i);
                    }
                }

                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {

                            Vector3Int neighbour = shortest.Coordinate + new Vector3Int(x, y, 0);

                            if (IsValidTile(neighbour)) {
                                //float tileCost = ((GroundData)(mapManager.GetTileData(MapManager.Layer.GroundLayer, neighbourCoordinate))).WalkCost;

                                float tileCost = mapManager.GetTileCost(neighbour);

                                bool inClosedList = false;
                                bool inOpenList = false;

                                foreach (PathNode node in closedList) {
                                    if (node.Coordinate == neighbour) {
                                        inClosedList = true;
                                        break;
                                    }
                                }

                                if (!inClosedList) {
                                    //  Might add some kind of heristic value later for tiebreaking
                                    float fScore = shortest.Fcost + tileCost;

                                    foreach (PathNode node in openList) {
                                        if (node.Coordinate == neighbour) {
                                            inOpenList = true;

                                            if (fScore < node.Fcost) {
                                                node.Fcost = fScore;
                                            }
                                            break;
                                        }
                                    }

                                    if (!inOpenList) {
                                        openList.Add(new PathNode(neighbour, fScore, 0, shortest));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return (null, -1);
        }

        public int GetPathIndexAtPosition(Vector3Int position) {
            for (int i = 0; i < currentPath.Count; i++) {
                if (currentPath[i] == position) {
                    return i;
                }
            }
            return -1;
        }

        public bool IsOnMainPath(Vector3Int position) {
            foreach (Vector3Int node in currentPath) {
                if (node == position) {
                    return true;
                }
            }
            return false;
        }

        public bool WouldBlockPath(Vector3Int position) {
            if (position == Vector3Int.FloorToInt(entrance.position) || position == Vector3Int.FloorToInt(exit.position)) {
                return true;
            }

            else if (IsOnMainPath(position)){
                List<PathNode> openList = new List<PathNode>();
                List<PathNode> closedList = new List<PathNode>();

                Vector3Int entranceCoordinate = Vector3Int.FloorToInt(entrance.transform.position);
                Vector3Int exitCoordinate = Vector3Int.FloorToInt(exit.transform.position);

                PathNode initialStep = new PathNode(entranceCoordinate);
                openList.Add(initialStep);

                while (openList.Count > 0) {
                    PathNode currentNode = openList[0];
                    foreach (PathNode node in openList) {
                        if (node.Fcost < currentNode.Fcost) {
                            currentNode = node;
                        }
                    }

                    PathNode parent = currentNode;
                    openList.Remove(currentNode);
                    closedList.Add(currentNode);
                    if (currentNode.Coordinate == exitCoordinate) {
                        return false;
                    }

                    for (int x = -1; x <= 1; x++) {
                        for (int y = -1; y <= 1; y++) {
                            if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                                Vector3Int neighbourCoordinate = parent.Coordinate + new Vector3Int(x, y, 0);
                                bool skipSuccessor = false;

                                if (IsValidTile(neighbourCoordinate) && neighbourCoordinate != position) {
                                    foreach (PathNode node in closedList) {
                                        if (node.Coordinate == neighbourCoordinate) {
                                            skipSuccessor = true;
                                            break;
                                        }
                                    }

                                    if (!skipSuccessor) {
                                        float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                                        float neighGCost = tileCost + parent.Gcost;
                                        float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                                        float neighFCost = neighGCost + neighHCost;

                                        foreach (PathNode node in openList) {
                                            if (node.Coordinate == neighbourCoordinate) {
                                                if (node.Fcost < neighFCost) {
                                                    node.Fcost = neighFCost;
                                                    node.ParentNode = parent;
                                                }
                                                skipSuccessor = true;
                                                break;
                                            }
                                        }

                                        if (!skipSuccessor) {
                                            PathNode successor = new PathNode(neighbourCoordinate, neighGCost, neighHCost, parent);
                                            openList.Add(successor);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            else {
                return false;
            }
        }
    }
}
