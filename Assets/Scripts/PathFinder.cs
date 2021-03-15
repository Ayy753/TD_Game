using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    [SerializeField]
    private Tilemap structureLayer;

    [SerializeField]
    private GameObject entrance;

    [SerializeField]
    private GameObject exit;

    private MapManager mapManager;
    private Vector3Int exitCoordinate;
    private Vector3Int entranceCoordinate;

    List<PathNode> openList;
    List<PathNode> closedList;

    /// <summary>
    /// The current path through the level
    /// </summary>
    public List<Vector3Int> Path { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Path Finder loaded");

        mapManager = GameManager.Instance.MapManager;
        exitCoordinate = structureLayer.WorldToCell(exit.transform.position);
        entranceCoordinate = structureLayer.WorldToCell(entrance.transform.position);

        //  Temporary debugging messages, used until unit testing is implemented 
        if (mapManager == null)
        {
            Debug.Log("mapmanager is null");
        }
        else if (entrance == null)
        {
            Debug.Log("entrance is null");
        }
        else if (exit == null)
        {
            Debug.Log("exit is null");
        }
        else
        {
            print(string.Format("entrance position:{0}, exit position:{1}", entranceCoordinate, exitCoordinate));
            FindPath(false);
        }
    }

    /// <summary>
    /// An object representing a node in a path chain
    /// </summary>
    protected class PathNode
    {
        //  This tiebreaker nudges the search algorithm towards the target slightly, improving calculation speed by a factor of 10
        //  by reducing the amount of exploration done
        //  **Note the tiebreaker wasn't implemented properly but "it just works"**
        //  **Attempts to implement it properly broke pathfinding**
        //  https://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#breaking-ties
        private float TieBreaker = 0.001f;
        public float HScore { get; private set; }
        public Vector3Int Coordinate { get; private set; }
        public PathNode Parent { get; private set; }
        private float _gScore;
        public float GScore
        {
            get
            {
                return _gScore * TieBreaker;
            }
            private set
            {
                _gScore = value;
            }
        }

        public float FScore
        {
            get { return GScore + HScore; }
        }

        public PathNode(Vector3Int coordinate, float gScore, float hScore, PathNode parent)
        {
            GScore = gScore;
            HScore = hScore;
            Coordinate = coordinate;
            Parent = parent;
        }

        //  initialize starting position
        public PathNode(Vector3Int startingPosition)
        {
            GScore = 0;
            HScore = 0;
            Coordinate = startingPosition;
            Parent = null;
        }

        public override string ToString()
        {
            return (string.Format("{0},{1}, FScore:{2}, HScore:{3}, GScore:{4}", Coordinate.x, Coordinate.y, FScore, HScore, GScore));
        }

        /// <summary>
        /// Prints the ToString method of each node in the path chain
        /// </summary>
        public void PrintChain()
        {
            print(this.ToString());
            if (Parent != null)
            {
                Parent.PrintChain();
            }
        }

        /// <summary>
        /// The start point to a recursive method that returns a list of tile coordinates representing a complete path
        /// </summary>
        /// <returns>List of path coordinates</returns>
        public List<Vector3Int> GetPath()
        {
            //  This is where we initialize the list
            //List<Vector3Int> pathCoords = new List<Vector3Int>();
            //return GetPath(pathCoords);

            List<Vector3Int> pathCoords = GetPath(new List<Vector3Int>());
            pathCoords.Reverse();
            return pathCoords;
        }

        /// <summary>
        /// A recursive method that passes a list to each parent in the chain for them to populate with their respective coordinates
        /// </summary>
        /// <param name="pathCoords"></param>
        /// <returns></returns>
        private List<Vector3Int> GetPath(List<Vector3Int> pathCoords)
        {
            pathCoords.Add(Coordinate);

            if (Parent != null)
            {
                Parent.GetPath(pathCoords);
            }
            return pathCoords;
        }
    }

    /// <summary>
    /// Estimates the shortest path between two points using a variation of the A* algorithm
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="highlightPath">Highlight tiles considered by pathfinder?</param>
    /// <returns>A chain of path nodes, or null if there is no valid path</returns>
    private PathNode CalculateShortestPath(Vector3Int start, Vector3Int end, bool highlightPath = false)
    {
        openList = new List<PathNode>();
        closedList = new List<PathNode>();

        //  a placeholder until I implement variable tile costs
        float tempTileCost = 1f;

        //  To prevent infinte loops from freezing unitiy
        int maxItt = 10000;
        int counter = 0;

        //  initialize with first item
        PathNode initialStep = new PathNode(start);
        openList.Add(initialStep);

        while (openList.Count > 0 && counter < maxItt)
        {
            //  find the lowest F score in openList
            PathNode lowestF = openList[0];
            foreach (PathNode node in openList)
            {
                if (node.FScore < lowestF.FScore)
                {
                    lowestF = node;
                }
            }

            //  Assign it as the parent to it's successors
            PathNode parent = lowestF;

            //  Remove parent from openList
            openList.Remove(lowestF);

            //  Add parent to closedList
            closedList.Add(lowestF);

            //  Get adjacent tiles
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                    if ((x == 0 && y != 0) || (x != 0 && y == 0))
                    {
                        Vector3Int tileCoordinate = parent.Coordinate + new Vector3Int(x, y, 0);
                        bool skipSuccessor = false;

                        //  Proceed if there is an open space at this position
                        if (IsValidTile(tileCoordinate))
                        {
                            // Skip this tile if its already been considered
                            foreach (PathNode node in closedList)
                            {
                                if (node.Coordinate == tileCoordinate)
                                {
                                    skipSuccessor = true;
                                    break;
                                }
                            }

                            //  Otherwise, process this tile 
                            if (!skipSuccessor)
                            {
                                float gScore = tempTileCost + parent.GScore;
                                float hScore = ManhattanDistance(tileCoordinate, exitCoordinate);
                                float fScore = gScore + hScore;

                                if (highlightPath)
                                {
                                    //  Tint tile yellow to visualize that the algorithm has considered it
                                    mapManager.TintTile(MapManager.Layer.GroundLayer, tileCoordinate, Color.yellow);
                                }

                                //  Return path chain if we found the exit tile
                                if (tileCoordinate == exitCoordinate)
                                {
                                    Debug.Log("found exit after " + counter + " iterations");
                                    PathNode foundPath = new PathNode(tileCoordinate, gScore, hScore, parent);
                                    if (highlightPath == true)
                                    {
                                        List<Vector3Int> pathCoords = foundPath.GetPath();
                                        mapManager.HighlightPath(pathCoords, Color.green);
                                    }

                                    return foundPath;
                                }

                                //  Check if openlist already contains a path to this tile
                                //  If it has, and the other one has a smaller F score, skip it
                                foreach (PathNode node in openList)
                                {
                                    if (node.Coordinate == tileCoordinate && node.FScore < fScore)
                                    {
                                        skipSuccessor = true;
                                        break;
                                    }
                                }

                                //  Otherwise add this successor to openList
                                if (!skipSuccessor)
                                {
                                    PathNode successor = new PathNode(tileCoordinate, gScore, hScore, parent);
                                    openList.Add(successor);
                                }
                            }

                        }
                    }
                }
            }
            counter++;
        }

        //  If no path is found, return null
        return null;
    }

    /// <summary>
    /// Finds and highlights the path solution through the unity inspector
    /// </summary>
    [ContextMenu("findPath")]
    private void FindPath()
    {
        FindPath(true);
        Debug.Log("Path size: " + Path.Count);
    }

    /// <summary>
    /// Used to calculate/update the current path through the level from the entrance to exit gates
    /// </summary>
    /// <param name="highlightPath"></param>
    private void FindPath(bool highlightPath)
    {
        PathNode bestPath = CalculateShortestPath(entranceCoordinate, exitCoordinate, highlightPath);
        Path = bestPath.GetPath();
    }

    /// <summary>
    /// A tile is valid if it doesn't contain a structure
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsValidTile(Vector3Int position)
    {
        return mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, position) != true;
    }

    /// <summary>
    /// Used to estimate the heuristic value of a tile
    /// </summary>
    /// <param name="start"></param>
    /// <param name="finish"></param>
    /// <returns></returns>
    private int ManhattanDistance(Vector3Int start, Vector3Int finish)
    {
        return Mathf.Abs(finish.x - start.x) + Mathf.Abs(finish.y - start.y);
    }
}
