using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    [SerializeField]
    private Tilemap structureLayer;

    [SerializeField]
    GameObject entrance;

    [SerializeField]
    GameObject exit;

    private MapManager mapManager;
    private Vector3Int exitCoordinate;
    private Vector3Int entranceCoordinate;

    List<PathStep> openList;
    List<PathStep> closedList;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Path Finder loaded");

        mapManager = GameManager.Instance.MapManager;
        exitCoordinate = structureLayer.WorldToCell(exit.transform.position);
        entranceCoordinate = structureLayer.WorldToCell(entrance.transform.position);

        if (mapManager == null)
        {
            print("mapmanager is null");
        }
        else if (entrance == null)
        {
            print("exit is null");
        }
        else
        {
            //CalculateShortestPath(structureLayer.WorldToCell(entrance.transform.position), exitCoordinate);
            print(string.Format("entrance position:{0}, exit position:{1}", entranceCoordinate, exitCoordinate));
        }
    }

    protected class PathStep
    {
        public int GScore { get; private set; }
        public int HScore { get; private set; }
        public Vector3Int Coordinate { get; private set; }
        public PathStep Parent { get; private set; }
        public int FScore
        {
            get { return GScore + HScore; }
        }

        public PathStep(Vector3Int coordinate, int gScore, int hScore, PathStep parent)
        {
            GScore = gScore;
            HScore = hScore;
            Coordinate = coordinate;
            Parent = parent;
        }

        //  initialize starting position
        public PathStep(Vector3Int startingPosition)
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

        public void PrintChain()
        {
            print(this.ToString());
            if (Parent != null)
            {
                Parent.PrintChain();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns>A chain of path nodes, or null if there is no valid path</returns>
    private PathStep CalculateShortestPath(Vector3Int start, Vector3Int end)
    {
        openList = new List<PathStep>();
        closedList = new List<PathStep>();

        //  a placeholder until I implement variable tile costs
        int tempTileCost = 1;

        //  To prevent infinte loops from freezing unitiy
        int maxItt = 10000;
        int counter = 0;

        //  initialize with first item
        PathStep initialStep = new PathStep(start);
        openList.Add(initialStep);

        while(openList.Count > 0 && counter < maxItt)
        {
            //  find the lowest F score in openList
            PathStep lowestF = openList[0];
            foreach (PathStep node in openList)
            {
                if (node.FScore < lowestF.FScore)
                {
                    lowestF = node;
                }
            }

            //  Tint this tile green to visually indicate the algorithm went through it
            mapManager.TintTile(MapManager.Layers.GroundLayer, lowestF.Coordinate, Color.green);

            //  Assign it as the parent to it's successors
            PathStep parent = lowestF;

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
                    //if ((x == 0 && y != 0) || (x != 0 && y == 0))
                    //{

                    Vector3Int tileCoordinate = parent.Coordinate + new Vector3Int(x, y, 0);
                    bool skipSuccessor = false;

                    //  Proceed if there is an open space at this position
                    if (IsValidTile(tileCoordinate))
                    {
                        // Skip this tile if its already been considered
                        foreach (PathStep node in closedList)
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
                            int gScore = tempTileCost + parent.GScore;
                            int hScore = ManhattanDistance(tileCoordinate, exitCoordinate);
                            int fScore = gScore + hScore;

                            //  Tint tile yellow to visualize that the algorithm has considered it
                            mapManager.TintTile(MapManager.Layers.GroundLayer, tileCoordinate, Color.yellow);

                            //  Return path chain if we found the exit tile
                            if (tileCoordinate == exitCoordinate)
                            {
                                print("found exit after " + counter + " iterations");
                                return new PathStep(tileCoordinate, gScore, hScore, parent);
                            }

                            //  Check if openlist already contains a path to this tile
                            //  If it has, and the other one has a smaller F score, skip it
                            foreach (PathStep node in openList)
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
                                PathStep successor = new PathStep(tileCoordinate, gScore, hScore, parent);
                                mapManager.TintTile(MapManager.Layers.GroundLayer, successor.Coordinate, Color.green);
                                openList.Add(successor);
                            }
                        }
                    
                        //}
                    }
                }
            }
            counter++;
        } 

        //  If no path is found, return null
        return null;
    }

    //  temp for debugging
    [ContextMenu("findPath")]
    private void FindPath()
    {
        CalculateShortestPath(structureLayer.WorldToCell(entrance.transform.position), exitCoordinate);
    }

    /// <summary>
    /// A tile is valid if it doesn't contain a structure
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsValidTile(Vector3Int position)
    {
        return  mapManager.ContainsTileAt(MapManager.Layers.StructureLayer, position) != true;
    }

    private int ManhattanDistance(Vector3Int start, Vector3Int finish)
    {
        return Mathf.Abs(finish.x - start.x) + Mathf.Abs(finish.y - start.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
