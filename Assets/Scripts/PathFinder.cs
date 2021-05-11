using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Responsible for all pathfinding operations
/// Currently in prototype phase
/// </summary>
public class PathFinder : MonoBehaviour
{
    private MapManager mapManager;

    [SerializeField]
    private Tilemap structureLayer;

    [SerializeField]
    private GameObject entrance;

    [SerializeField]
    private GameObject exit;

    private Vector3Int exitCoordinate;
    private Vector3Int entranceCoordinate;

    public delegate void PathRecalculated(List<Vector3Int> newPath);
    public delegate void PathRecalculating();
    public delegate void InitialPathCalculated();
    public static PathRecalculated OnPathRecalculated;
    public static PathRecalculating OnPathRecalculating;
    public static InitialPathCalculated OnInitialPathCalculated;

    private string FilePath = "Assets/LevelData/PathData/demo_path_data.txt";


    /// <summary>
    /// The current path through the map
    /// </summary>
    public List<Vector3Int> CurrentPath { get; protected set; }
    public List<Vector3Int> PreviousPath { get; protected set; }

    private void Start()
    {
        Debug.Log("Path Finder loaded");

        mapManager = GameManager.Instance.MapManager;
        exitCoordinate = structureLayer.WorldToCell(exit.transform.position);
        entranceCoordinate = structureLayer.WorldToCell(entrance.transform.position);

        //  If game is not running in editor, load intital path data from file
        if (Application.isEditor == false)
        {
            LoadPathData();
            mapManager.HighlightPath(CurrentPath, Color.cyan);
            if (OnInitialPathCalculated != null)
            {
                OnInitialPathCalculated.Invoke();
            }
        }
        //  Otherwise calculate the inital path
        else
        {
            StartCoroutine("CalculateShortestPath");
        }
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
        public float Hcost { get; private set; }

        //  Distance from start node
        public float Gcost { get; private set; }

        //  Combined distances
        public float Fcost
        {
            get { return Gcost + Hcost; }
            set {; }
        }
        public Vector3Int Coordinate { get; private set; }
        public PathNode ParentNode { get; set; }

        public PathNode(Vector3Int coordinate, float gScore, float hScore, PathNode parent)
        {
            Gcost = gScore;
            Hcost = hScore;
            Coordinate = coordinate;
            ParentNode = parent;
        }

        //  initialize starting position
        public PathNode(Vector3Int startingPosition)
        {
            Gcost = 0;
            Hcost = 0;
            Coordinate = startingPosition;
            ParentNode = null;
        }

        public override string ToString()
        {
            return (string.Format("{0},{1}, FScore:{2}, HScore:{3}, GScore:{4}", Coordinate.x, Coordinate.y, Fcost, Hcost, Gcost));
        }

        /// <summary>
        /// Prints the ToString method of each node in the path chain
        /// </summary>
        public void PrintChain()
        {
            print(this.ToString());
            if (ParentNode != null)
            {
                ParentNode.PrintChain();
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

            if (ParentNode != null)
            {
                ParentNode.GetPath(pathCoords);
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
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        int counter = 0;

        mapManager.UnhighlightAll();

        //  initialize with first item
        PathNode initialStep = new PathNode(entranceCoordinate);
        openList.Add(initialStep);

        while (openList.Count > 0)
        {
            //  find the lowest F cost in openList
            PathNode currentNode = openList[0];
            foreach (PathNode node in openList)
            {
                if (node.Fcost < currentNode.Fcost)
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
                PathNode foundPath = new PathNode(currentNode.Coordinate, currentNode.Gcost, currentNode.Hcost, parent);
                
                CurrentPath = foundPath.GetPath();
                mapManager.HighlightPath(CurrentPath, Color.cyan);

                if (PreviousPath != null && OnPathRecalculated != null)
                {
                    OnPathRecalculated.Invoke(CurrentPath);
                }       
                //  If this is the initial path calculation
                else if (PreviousPath == null)
                {
                    //  Save path to file only if game is running in the editor
                    if (Application.isEditor)
                    {
                        SavePathData();
                    }

                    //  Alert other objects that the main path has been calculated
                    if (OnInitialPathCalculated != null)
                    {
                        OnInitialPathCalculated.Invoke();
                    }
                }
                //foundPath.PrintChain();
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
                                float tileCost = mapManager.GetTileCost(neighbourCoordinate);
                                float neighGCost = tileCost + parent.Gcost;
                                float neighHCost = ManhattanDistance(neighbourCoordinate, exitCoordinate);
                                float neighFCost = neighGCost + neighHCost;

                                //mapManager.HighlightTile(MapManager.Layer.GroundLayer, neighbourCoordinate, Color.yellow);

                                //  Check if openlist already contains a path to this tile
                                //  If it has, and the other one has a smaller F cost, update cost and parent
                                foreach (PathNode node in openList)
                                {
                                    if (node.Coordinate == neighbourCoordinate)
                                    {
                                        if (node.Fcost < neighFCost)
                                        {
                                            node.Fcost = neighFCost;
                                            node.ParentNode = parent;
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

                        //  Pause coroutine every 75th tile processed to allow other processes to run in the meantime
                        //  Using a resetting counter is probably a lot faster than the modulus operator 
                        counter++;
                        if (counter > 75)
                        {
                            counter = 0;
                            yield return null;
                        }
                    }
                }
            }
        }
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

    /// <summary>
    /// Handle pathfinding when a structure is built or demolished
    /// </summary>
    /// <param name="demolish"></param>
    private void HandleStructureChanged(bool demolish)
    {
        //  If a structure demolished
        //  or a structure was built that blocks the current path
        //  Recalculate path
        if (demolish == true || PathBlocked(CurrentPath) == true)
        {
            StopCoroutine("CalculateShortestPath");
            PreviousPath = CurrentPath;
            StartCoroutine("CalculateShortestPath");
            if (OnPathRecalculating != null)
            {
                OnPathRecalculating.Invoke();
            }
        }
    }

    /// <summary>
    /// Saves the path coordinates to file
    /// </summary>
    private void SavePathData()
    {
        using (StreamWriter sw = new StreamWriter(FilePath, false))
        {
            foreach (Vector3Int node in CurrentPath)
            {
                sw.WriteLine(string.Format("{0},{1},{2}", node.x, node.y, node.z));
            }
        }
    }

    /// <summary>
    /// Loads the path coordinates from file
    /// </summary>
    private void LoadPathData()
    {
        CurrentPath = new List<Vector3Int>();
        using (StreamReader sr = new StreamReader(FilePath))
        {
            while (sr.EndOfStream == false)
            {
                string line = sr.ReadLine();
                if (line != string.Empty)
                {
                    string[] chars = line.Split(',');
                    CurrentPath.Add(new Vector3Int(int.Parse(chars[0]), int.Parse(chars[1]), int.Parse(chars[2])));
                }
            }
        }
    }

    /// <summary>
    /// Performs a flood search for the shortest route to the main path
    /// </summary>
    public (List<Vector3Int>, int) RouteToPath(Vector3Int startingPos)
    {
        List<PathNode> closedList = new List<PathNode>();
        List<PathNode> openList = new List<PathNode>();

        PathNode parentNode = new PathNode(startingPos);
        openList.Add(parentNode);

        int counter = 0;
        int maxItt = 1000;

        while (openList.Count > 0 && counter < maxItt)
        {
            parentNode = openList[0];

            //  Select the lowest cost from the starting position in the openlist
            foreach (PathNode node in openList)
            {
                if (node.Fcost < parentNode.Fcost)
                {
                    parentNode = node;
                }
            }

            openList.Remove(parentNode);
            closedList.Add(parentNode);

            for (int i = 0; i < CurrentPath.Count; i++)
            {
                //  If it is we found the shortest route to the main path
                if (CurrentPath[i] == parentNode.Coordinate)
                {
                    List<Vector3Int> path = parentNode.GetPath();

                    //mapManager.HighlightPath(path, Color.grey);
                    return (path, i);
                }
            }

            //  Examine the neighbours
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    //  Ignore diagonal tiles
                    if ((x == 0 && y != 0) || (x != 0 && y == 0))
                    {
                        Vector3Int neighbourCoordinate = parentNode.Coordinate + new Vector3Int(x, y, 0);
                        if (IsValidTile(neighbourCoordinate))
                        {
                            float tileCost = ((GroundData)(mapManager.GetTileData(MapManager.Layer.GroundLayer, neighbourCoordinate))).WalkCost;
                            bool inClosedList = false;
                            bool inOpenList = false;

                            foreach (PathNode node in closedList)
                            {
                                if (node.Coordinate == neighbourCoordinate)
                                {
                                    inClosedList = true;
                                    break;
                                }
                            }

                            if (inClosedList == false)
                            {
                                //  Might add some kind of heristic value later for tiebreaking
                                float fScore = parentNode.Fcost + tileCost;

                                foreach (PathNode node in openList)
                                {
                                    if (node.Coordinate == neighbourCoordinate)
                                    {
                                        inOpenList = true;

                                        if (fScore < node.Fcost)
                                        {
                                            node.Fcost = fScore;
                                        }
                                        break;
                                    }
                                }

                                if (inOpenList == false)
                                {
                                    openList.Add(new PathNode(neighbourCoordinate, fScore, 0, parentNode));
                                    //mapManager.HighlightTile(MapManager.Layer.GroundLayer, neighbourCoordinate, Color.yellow);
                                }
                            }
                        }
                    }
                }
            }
            counter++;
        }
        Debug.Log("Did not find route to path");
        return (null, -1);
    }

    /// <summary>
    /// Check if any tile in the current path is blocked
    /// </summary>
    /// <returns></returns>
    public bool PathBlocked(List<Vector3Int> path)
    {
        foreach (Vector3Int node in path)
        {
            if (IsValidTile(node) == false)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Is this point a part of the main path?
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool PointOnPath(Vector3Int point)
    {
        foreach (Vector3Int node in CurrentPath)
        {
            if (node == point)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Highlights the path in cyan
    /// </summary>
    public void HighlightPath()
    {
        mapManager.HighlightPath(CurrentPath, Color.cyan);
    }

    /// <summary>
    /// Remove the tinting from the tiles
    /// </summary>
    public void UnhighlightPath()
    {
        mapManager.UnhighlightAll();
    }
}
