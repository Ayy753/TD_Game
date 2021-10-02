namespace DefaultNamespace.TilemapSystem {
    using System.Collections.Generic;
    using UnityEngine;

    class PathNode {
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

    public class PathCalculator {
        private readonly IMapManager mapManager;
        
        private Vector3Int entranceCoordinate;
        private Vector3Int exitCoordinate;
        private List<PathNode> openList;
        private List<PathNode> closedList;
        private List<Vector3Int> nodeList;

        public enum PathCondition {
            None,
            StructureBuilt,
            StructureDemolished,
            PlatformBuilt
        }

        public PathCalculator(IMapManager mapManager, Vector3Int entranceCoordinate, Vector3Int exitCoordinate) {
            this.mapManager = mapManager;
            this.entranceCoordinate = entranceCoordinate;
            this.exitCoordinate = exitCoordinate;
            List<Vector3Int> nodeList = CalculatePath();
        }

        public List<Vector3Int> CalculatePath() {
            InitializePath();

            while (openList.Count > 0) {
                PathNode parent = LowestFScoreInOpenlist();

                openList.Remove(parent);
                closedList.Add(parent);

                //  Return path chain if we found the exit tile
                if (parent.Coordinate == exitCoordinate) {
                    PathNode foundPath = new PathNode(parent.Coordinate, parent.Gcost, parent.Hcost, parent);
                    return foundPath.GetPath();
                }

                //  Process neighbouring tiles
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                            ProcessNeighbour(new Vector3Int(x, y, 0), parent);
                        }
                    }
                }
            }
            return null;
        }

        public List<Vector3Int> GetBuildPreviewPath(Vector3Int positionBlocked) {
            InitializePath();

            while (openList.Count > 0) {
                PathNode parent = LowestFScoreInOpenlist();

                openList.Remove(parent);
                closedList.Add(parent);

                //  Return path chain if we found the exit tile
                if (parent.Coordinate == exitCoordinate) {
                    PathNode foundPath = new PathNode(parent.Coordinate, parent.Gcost, parent.Hcost, parent);
                    return foundPath.GetPath();
                }

                //  Process neighbouring tiles
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                            ProcessNeighbourForBuildPreview(new Vector3Int(x, y, 0), parent, positionBlocked);
                        }
                    }
                }
            }
            return null;
        }

        public List<Vector3Int> GetDemolishPreviewPath(Vector3Int positionDemolished) {
            InitializePath();

            while (openList.Count > 0) {
                PathNode parent = LowestFScoreInOpenlist();

                openList.Remove(parent);
                closedList.Add(parent);

                //  Return path chain if we found the exit tile
                if (parent.Coordinate == exitCoordinate) {
                    PathNode foundPath = new PathNode(parent.Coordinate, parent.Gcost, parent.Hcost, parent);
                    return foundPath.GetPath();
                }

                //  Process neighbouring tiles
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                            ProcessNeighbourForDemolishPreivew(new Vector3Int(x, y, 0), parent, positionDemolished);
                        }
                    }
                }
            }
            return null;
        }

        public List<Vector3Int> GetPlatformBuiltPreviewPath(Vector3Int positionBuilt, float platformMovementCost) {
            InitializePath();

            while (openList.Count > 0) {
                PathNode parent = LowestFScoreInOpenlist();

                openList.Remove(parent);
                closedList.Add(parent);

                //  Return path chain if we found the exit tile
                if (parent.Coordinate == exitCoordinate) {
                    PathNode foundPath = new PathNode(parent.Coordinate, parent.Gcost, parent.Hcost, parent);
                    return foundPath.GetPath();
                }

                //  Process neighbouring tiles
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                        if ((x == 0 && y != 0) || (x != 0 && y == 0)) {
                            ProcessNeighbourForPlatformBuildPreview(new Vector3Int(x, y, 0), parent, positionBuilt, platformMovementCost);
                        }
                    }
                }
            }
            return null;
        }

        private void InitializePath() {
            openList = new List<PathNode>();
            closedList = new List<PathNode>();

            PathNode initialStep = new PathNode(entranceCoordinate);
            openList.Add(initialStep);
        }

        private PathNode LowestFScoreInOpenlist() {
            PathNode currentNode = openList[0];
            foreach (PathNode node in openList) {
                if (node.Fcost < currentNode.Fcost) {
                    currentNode = node;
                }
            }
            return currentNode;
        }

        private void ProcessNeighbour(Vector3Int position, PathNode parent) {
            Vector3Int neighbourCoordinate = parent.Coordinate + position;

            if (IsValidTile(neighbourCoordinate) && !IsPositionInClosedList(neighbourCoordinate)) {
                float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                float neighGCost = tileCost + parent.Gcost;
                float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                float neighFCost = neighGCost + neighHCost;

                bool isInOpenList = CheckIfPositionIsInOpenListAndUpdateFScore(neighbourCoordinate, parent, neighFCost);

                if (!isInOpenList) {
                    PathNode successor = new PathNode(neighbourCoordinate, neighGCost, neighHCost, parent);
                    openList.Add(successor);
                }
            }
        }

        private void ProcessNeighbourForBuildPreview(Vector3Int position, PathNode parent, Vector3Int positionBlocked) {
            Vector3Int neighbourCoordinate = parent.Coordinate + position;

            if (IsValidTile(neighbourCoordinate) && neighbourCoordinate != positionBlocked) {
                float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                float neighGCost = tileCost + parent.Gcost;
                float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                float neighFCost = neighGCost + neighHCost;

                bool isInOpenList = CheckIfPositionIsInOpenListAndUpdateFScore(neighbourCoordinate, parent, neighFCost);

                if (!isInOpenList) {
                    PathNode successor = new PathNode(neighbourCoordinate, neighGCost, neighHCost, parent);
                    openList.Add(successor);
                }
            }
        }

        private void ProcessNeighbourForDemolishPreivew(Vector3Int position, PathNode parent, Vector3Int positionDemolished) {
            Vector3Int neighbourCoordinate = parent.Coordinate + position;

            if (IsValidTile(neighbourCoordinate) || neighbourCoordinate == positionDemolished) {
                float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                float neighGCost = tileCost + parent.Gcost;
                float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                float neighFCost = neighGCost + neighHCost;

                bool isInOpenList = CheckIfPositionIsInOpenListAndUpdateFScore(neighbourCoordinate, parent, neighFCost);

                if (!isInOpenList) {
                    PathNode successor = new PathNode(neighbourCoordinate, neighGCost, neighHCost, parent);
                    openList.Add(successor);
                }
            }
        }

        private void ProcessNeighbourForPlatformBuildPreview(Vector3Int position, PathNode parent, Vector3Int positionBuilt, float platformMovementCost) {
            Vector3Int neighbourCoordinate = parent.Coordinate + position;

            if (IsValidTile(neighbourCoordinate) && !IsPositionInClosedList(neighbourCoordinate)) {
                float tileCost;
                if (neighbourCoordinate != positionBuilt) {
                    tileCost = mapManager.GetTileCost(neighbourCoordinate);
                }
                else {
                    tileCost = platformMovementCost;
                }
                float neighGCost = tileCost + parent.Gcost;
                float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                float neighFCost = neighGCost + neighHCost;

                bool isInOpenList = CheckIfPositionIsInOpenListAndUpdateFScore(neighbourCoordinate, parent, neighFCost);

                if (!isInOpenList) {
                    PathNode successor = new PathNode(neighbourCoordinate, neighGCost, neighHCost, parent);
                    openList.Add(successor);
                }
            }
        }

        private bool IsValidTile(Vector3Int position) {
            if (!mapManager.ContainsTileAt(MapLayer.StructureLayer, position) &&
                mapManager.ContainsTileAt(MapLayer.GroundLayer, position)) {
                return true;
            }
            else {
                return false;
            }
        }

        private int ManhattanDistance(Vector3Int start, Vector3Int finish) {
            return Mathf.Abs(finish.x - start.x) + Mathf.Abs(finish.y - start.y);
        }

        private bool IsPositionInClosedList(Vector3Int position) {
            foreach (PathNode node in closedList) {
                if (node.Coordinate == position) {
                    return true;
                }
            }
            return false;
        }

        private bool CheckIfPositionIsInOpenListAndUpdateFScore(Vector3Int position, PathNode parent, float neighFCost) {
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
    }
}
