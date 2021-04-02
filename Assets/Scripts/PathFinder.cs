using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Responsible for all pathfinding operations
/// Currently in prototype phase
/// </summary>
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

    private List<PathNode> openList;
    private List<PathNode> closedList;

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

        StartCoroutine("CalculateShortestPath");
    }

    private void OnEnable()
    {
        MapManager.OnStructureChanged += HandleStructureChanged;
    }
    private void OnDisable()
    {
        MapManager.OnStructureChanged -= HandleStructureChanged;
    }

    /// <summary>
    /// An object representing a node in a path chain
    /// </summary>
    protected class PathNode
    {
        //  https://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#breaking-ties

        //  Estimated distance from end node
        public float HScore { get; private set; }

        //  Distance from start node
        public float GScore { get; private set; }

        //  Combined distances
        public float FScore
        {
            get { return GScore + HScore; }
            set {; }
        }
        public Vector3Int Coordinate { get; private set; }
        public PathNode Parent { get; set; }

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
    private IEnumerator CalculateShortestPath()
    {
        openList = new List<PathNode>();
        closedList = new List<PathNode>();

        //  a placeholder until I implement variable tile costs
        float tempTileCost = 1f;

        //  To prevent infinte loops from freezing unitiy
        int maxItt = 10000;
        int counter = 0;

        mapManager.ClearAllTints();

        //  initialize with first item
        PathNode initialStep = new PathNode(entranceCoordinate);
        openList.Add(initialStep);

        while (openList.Count > 0 && counter < maxItt)
        {
            //  find the lowest F cost in openList
            PathNode currentNode = openList[0];
            foreach (PathNode node in openList)
            {
                if (node.FScore < currentNode.FScore)
                {
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
            if (currentNode.Coordinate == exitCoordinate)
            {
                Debug.Log("found exit after " + counter + " iterations");
                PathNode foundPath = new PathNode(currentNode.Coordinate, currentNode.FScore, currentNode.GScore, parent);
                
                Path = foundPath.GetPath();
                mapManager.HighlightPath(Path, Color.cyan);
                yield break;
            }

            //  Process neighbouring tiles
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //  Ignore diagonal tiles (one of the coords must be zero and the other non-zero in order to be non-diagonal)
                    if ((x == 0 && y != 0) || (x != 0 && y == 0))
                    {
                        Vector3Int neighbourCoordinate = parent.Coordinate + new Vector3Int(x, y, 0);
                        bool skipSuccessor = false;

                        //  Proceed if there is an open space at this position
                        if (IsValidTile(neighbourCoordinate))
                        {
                            // Skip this tile if its already been considered
                            foreach (PathNode node in closedList)
                            {
                                if (node.Coordinate == neighbourCoordinate)
                                {
                                    skipSuccessor = true;
                                    break;
                                }
                            }

                            //  Otherwise, process this tile 
                            if (skipSuccessor == false)
                            {
                                float neighGCost = tempTileCost + parent.GScore;
                                float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                                float neighFCost = neighGCost + neighHCost;

                                mapManager.HighlightTile(MapManager.Layer.GroundLayer, neighbourCoordinate, Color.yellow);

                                //  Check if openlist already contains a path to this tile
                                //  If it has, and the other one has a smaller F cost, update cost and parent
                                foreach (PathNode node in openList)
                                {
                                    if (node.Coordinate == neighbourCoordinate)
                                    {
                                        if (node.FScore < neighFCost)
                                        {
                                            node.FScore = neighFCost;
                                            node.Parent = parent;
                                        }
                                        skipSuccessor = true;
                                        break;
                                    }
                                }

                                //  Otherwise add this successor to openList
                                if (skipSuccessor == false)
                                {
                                    PathNode successor = new PathNode(neighbourCoordinate, neighGCost, neighHCost, parent);
                                    openList.Add(successor);
                                }
                            }
                        }
                    }
                }
            }
            counter++;
            //  Pause coroutine every 45th iteration to allow other processes to run in the meantime
            if (counter % 45 == 0)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// Highlights the path in cyan
    /// </summary>
    public void HighlightPath()
    {
        mapManager.HighlightPath(Path, Color.cyan);
    }

    /// <summary>
    /// Remove the tinting from the tiles
    /// </summary>
    public void UnhighlightPath()
    {
        mapManager.ClearAllTints();
    }

    /// <summary>
    /// A tile is valid(walkable) if it doesn't contain a structure, but contains a ground tile
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsValidTile(Vector3Int position)
    {
        if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, position) != true &&
            mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, position) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
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

    private void HandleStructureChanged()
    {
        StopCoroutine("CalculateShortestPath");
        StartCoroutine("CalculateShortestPath");
    }
}
