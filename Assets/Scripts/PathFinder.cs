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


    List<PathStep> openList;
    List<PathStep> closedList;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Path Finder loaded");

        mapManager = GameManager.Instance.MapManager;

        openList = new List<PathStep>();
        closedList = new List<PathStep>();

        exitCoordinate = structureLayer.WorldToCell(exit.transform.position);

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
            print(string.Format("entrance position:{0}, exit position:{1}", structureLayer.WorldToCell(entrance.transform.position), exitCoordinate));
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
            return (string.Format("{0}{1}, FScore:{2}, HScore:{3}, GScore:{4}", Coordinate.x, Coordinate.y, FScore, HScore, GScore));
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
        PathStep initialStep = new PathStep(start);

        //  a placeholder until I implement variable tile costs
        int tempTileCost = 1;
        openList.Add(initialStep);


        //temp for debugging
        int maxItt = 1000;
        int counter = 0;

        do
        {
            //  initialize with first item
            PathStep lowestF = openList[0];

            //  find the lowest F score in openList
            foreach (PathStep node in openList)
            {
                if (node.FScore < lowestF.FScore)
                {
                    lowestF = node;
                }
            }

            // Get adjacent tiles
            for (int x = -1; x <= 1; x+=2)
            {
                for (int y = -1; y <= 1; y+=2)
                {
                    Vector3Int tileCoordinate = lowestF.Coordinate + new Vector3Int(x, y, 0);
                    int fScore = tempTileCost + lowestF.GScore;
                    int hScore = ManhattanDistance(tileCoordinate, exitCoordinate);
                    bool skipSuccessor = false;

                    print(string.Format("checking tile {0},{1}", tileCoordinate.x, tileCoordinate.y));


                    //  if there is a open space at this position
                    if (IsValidTile(tileCoordinate))
                    {
                        //  Return path chain if we found the exit tile
                        if (tileCoordinate == exitCoordinate)
                        {
                            print("found exit");
                            return new PathStep(tileCoordinate, fScore, hScore, lowestF);
                        }

                        //  Otherwise search openlist to ensure there isn't a shorter f score in the same position
                        foreach (PathStep openNode in openList)
                        {
                            if (openNode.Coordinate == tileCoordinate && openNode.FScore < fScore)
                            {
                                print("a shorter path was found in openlist");
                                skipSuccessor = true;
                                break;
                            }
                        }

                        //  search closed list to ensure there isn't a shorter f score in the same position
                        if (!skipSuccessor)
                        {
                            foreach (PathStep closedNode in closedList)
                            {
                                if (closedNode.Coordinate == tileCoordinate && closedNode.FScore < fScore)
                                {
                                    print("a shorter path was found in closedlist");
                                    skipSuccessor = true;
                                    break;
                                }
                            }
                        }

                        if (!skipSuccessor)
                        {
                            //  otherwise add this successor to openList
                            PathStep successor = new PathStep(tileCoordinate, fScore, hScore, lowestF);
                            print("a successor was found: " + successor.ToString());
                            openList.Add(successor);
                        }

                    }
                }
            }

            //  Add lowest to closedList
            closedList.Add(lowestF);
            print("Added " + lowestF.ToString() + " to closedlist");
            //  remove lowest from openList
            openList.RemoveAt(openList.IndexOf(lowestF));


            print("openlist count: " + openList.Count);


            counter++;
            print(counter);
        } while (openList.Count > 0 && counter < maxItt);

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
