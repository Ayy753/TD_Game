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

        public event EventHandler PathRecalculated;

        public void Initialize() {
            Debug.Log("Initializing pathfinder");

            entrance = GameObject.Find("Entrance").transform;
            exit = GameObject.Find("Exit").transform;

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
            if (!mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position) &&
                mapManager.ContainsTileAt(IMapManager.Layer.GroundLayer, position)) {
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
            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            Vector3Int entranceCoordinate = Vector3Int.FloorToInt(entrance.transform.position);
            Vector3Int exitCoordinate = Vector3Int.FloorToInt(exit.transform.position);

            //  initialize with entrance coordinate
            PathNode initialStep = new PathNode(entranceCoordinate);
            openList.Add(initialStep);

            while (openList.Count > 0) {
                //  find the lowest F cost in openList
                PathNode currentNode = openList[0];
                foreach (PathNode node in openList) {
                    if (node.Fcost < currentNode.Fcost) {
                        currentNode = node;
                    }
                }

                //  Assign it as the parent to it's successors
                PathNode parent = currentNode;

                //  Remove parent from openList
                openList.Remove(currentNode);

                //  Add parent to closedList
                closedList.Add(currentNode);

                //  Return path chain if we found the exit tile
                if (currentNode.Coordinate == exitCoordinate) {
                    PathNode foundPath = new PathNode(currentNode.Coordinate, currentNode.Gcost, currentNode.Hcost, parent);
                    currentPath = foundPath.GetPath();
                    PathRecalculated.Invoke(this, EventArgs.Empty);
                    break;
                }

                //  Process neighbouring tiles
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                            ProcessTile(new Vector3Int(x, y, 0), parent, exitCoordinate, openList, closedList);
                        }
                    }
                }
            }
        }

        private void ProcessTile(Vector3Int position, PathNode parent, Vector3Int exitCoordinate, List<PathNode> openList, List<PathNode> closedList) {
            Vector3Int neighbourCoordinate = parent.Coordinate + position;

            if (IsValidTile(neighbourCoordinate) && !IsPositionInClosedList(neighbourCoordinate, closedList)) {
                float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                float neighGCost = tileCost + parent.Gcost;
                float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                float neighFCost = neighGCost + neighHCost;

                bool isInOpenList = CheckIfPositionIsInOpenListAndUpdateFScore(neighbourCoordinate, parent, openList, neighFCost);

                if (!isInOpenList) {
                    PathNode successor = new PathNode(neighbourCoordinate, neighGCost, neighHCost, parent);
                    openList.Add(successor);
                }
            }
        }

        private bool IsPositionInClosedList(Vector3Int position, List<PathNode> closedList) {
            foreach (PathNode node in closedList) {
                if (node.Coordinate == position) {
                    return true;
                }
            }
            return false;
        }

        private bool CheckIfPositionIsInOpenListAndUpdateFScore(Vector3Int position, PathNode parent, List<PathNode> openList, float neighFCost) {
            foreach (PathNode node in openList) {
                if (node.Coordinate == position) {
                    if (node.Fcost < neighFCost) {
                        node.Fcost = neighFCost;
                        node.ParentNode = parent;
                    }
                    return true;
                }
            }
            return false;
        }

        public List<Vector3Int> GetBuildPreviewPath(Vector3Int positionBlocked) {
            Debug.Log("Calculating build preview path");

            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            Vector3Int entranceCoordinate = Vector3Int.FloorToInt(entrance.transform.position);
            Vector3Int exitCoordinate = Vector3Int.FloorToInt(exit.transform.position);

            Debug.Log(string.Format("entrance coordinate: {0}, exit coordinate: {1}", entranceCoordinate, exitCoordinate));

            //  initialize with entrance coordinate
            PathNode initialStep = new PathNode(entranceCoordinate);
            openList.Add(initialStep);

            while (openList.Count > 0) {
                //  find the lowest F cost in openList
                PathNode currentNode = openList[0];
                foreach (PathNode node in openList) {
                    if (node.Fcost < currentNode.Fcost) {
                        currentNode = node;
                    }
                }

                //  Assign it as the parent to it's successors
                PathNode parent = currentNode;

                //  Remove parent from openList
                openList.Remove(currentNode);

                //  Add parent to closedList
                closedList.Add(currentNode);

                //  Return path chain if we found the exit tile
                if (currentNode.Coordinate == exitCoordinate) {
                    PathNode foundPath = new PathNode(currentNode.Coordinate, currentNode.Gcost, currentNode.Hcost, parent);
                    PathRecalculated.Invoke(this, EventArgs.Empty);
                    return foundPath.GetPath();
                }

                //  Process neighbouring tiles
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                            Vector3Int neighbourCoordinate = parent.Coordinate + new Vector3Int(x, y, 0);
                            bool skipSuccessor = false;

                            //  Proceed if there is an open space at this position
                            if (IsValidTile(neighbourCoordinate) && neighbourCoordinate != positionBlocked) {
                                // Skip this tile if its already been considered
                                foreach (PathNode node in closedList) {
                                    if (node.Coordinate == neighbourCoordinate) {
                                        skipSuccessor = true;
                                        break;
                                    }
                                }

                                //  Otherwise, process this tile 
                                if (!skipSuccessor) {
                                    float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                                    float neighGCost = tileCost + parent.Gcost;
                                    float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                                    float neighFCost = neighGCost + neighHCost;

                                    //  Check if openlist already contains a path to this tile
                                    //  If it has, and the other one has a smaller F cost, update cost and parent
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

                                    //  Otherwise add this successor to openList
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
            return null;
        }

        public List<Vector3Int> GetDemolishPreviewPath(Vector3Int positionDemolished) {
            Debug.Log("Calculating demolish preview path");

            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            Vector3Int entranceCoordinate = Vector3Int.FloorToInt(entrance.transform.position);
            Vector3Int exitCoordinate = Vector3Int.FloorToInt(exit.transform.position);

            Debug.Log(string.Format("entrance coordinate: {0}, exit coordinate: {1}", entranceCoordinate, exitCoordinate));

            //  initialize with entrance coordinate
            PathNode initialStep = new PathNode(entranceCoordinate);
            openList.Add(initialStep);

            while (openList.Count > 0) {
                //  find the lowest F cost in openList
                PathNode currentNode = openList[0];
                foreach (PathNode node in openList) {
                    if (node.Fcost < currentNode.Fcost) {
                        currentNode = node;
                    }
                }

                //  Assign it as the parent to it's successors
                PathNode parent = currentNode;

                //  Remove parent from openList
                openList.Remove(currentNode);

                //  Add parent to closedList
                closedList.Add(currentNode);

                //  Return path chain if we found the exit tile
                if (currentNode.Coordinate == exitCoordinate) {
                    PathNode foundPath = new PathNode(currentNode.Coordinate, currentNode.Gcost, currentNode.Hcost, parent);
                    PathRecalculated.Invoke(this, EventArgs.Empty);
                    return foundPath.GetPath();
                }

                //  Process neighbouring tiles
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                            Vector3Int neighbourCoordinate = parent.Coordinate + new Vector3Int(x, y, 0);
                            bool skipSuccessor = false;

                            //  Proceed if there is an open space at this position
                            if (IsValidTile(neighbourCoordinate) || neighbourCoordinate == positionDemolished) {
                                // Skip this tile if its already been considered
                                foreach (PathNode node in closedList) {
                                    if (node.Coordinate == neighbourCoordinate) {
                                        skipSuccessor = true;
                                        break;
                                    }
                                }

                                //  Otherwise, process this tile 
                                if (!skipSuccessor) {
                                    float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                                    float neighGCost = tileCost + parent.Gcost;
                                    float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                                    float neighFCost = neighGCost + neighHCost;

                                    //  Check if openlist already contains a path to this tile
                                    //  If it has, and the other one has a smaller F cost, update cost and parent
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

                                    //  Otherwise add this successor to openList
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
            return null;
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

        /// <summary>
        /// An object representing a node in a path chain
        /// </summary>
        protected class PathNode {
            //  Estimated distance from end node
            public float Hcost { get; private set; }

            //  Distance from start node
            public float Gcost { get; private set; }

            //  Combined distances
            public float Fcost {
                get { return Gcost + Hcost; }
                set {; }
            }
            public Vector3Int Coordinate { get; private set; }

            public PathNode ParentNode { get; set; }

            public PathNode(Vector3Int coordinate, float gScore, float hScore, PathNode parent) {
                Gcost = gScore;
                Hcost = hScore;
                Coordinate = coordinate;
                ParentNode = parent;
            }

            //  initialize starting position
            public PathNode(Vector3Int startingPosition) {
                Gcost = 0;
                Hcost = 0;
                Coordinate = startingPosition;
                ParentNode = null;
            }

            public override string ToString() {
                return (string.Format("{0},{1}, FScore:{2}, HScore:{3}, GScore:{4}", Coordinate.x, Coordinate.y, Fcost, Hcost, Gcost));
            }

            /// <summary>
            /// The start point to a recursive method that returns a list of tile coordinates representing a complete path
            /// </summary>
            /// <returns>List of path coordinates</returns>
            public List<Vector3Int> GetPath() {
                //  This is where we initialize the list
                List<Vector3Int> pathCoords = GetPath(new List<Vector3Int>());
                pathCoords.Reverse();
                return pathCoords;
            }

            /// <summary>
            /// A recursive method that passes a list to each parent in the chain for them to populate with their respective coordinates
            /// </summary>
            /// <param name="pathCoords"></param>
            /// <returns></returns>
            private List<Vector3Int> GetPath(List<Vector3Int> pathCoords) {
                pathCoords.Add(Coordinate);

                if (ParentNode != null) {
                    ParentNode.GetPath(pathCoords);
                }
                return pathCoords;
            }
        }
    }
}
