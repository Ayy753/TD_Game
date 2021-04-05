using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// Generates a maze using data I created externally, for the purpose of testing pathfinding
/// It uses a maze data file I created from an image.
/// I converted the image into 1bit color data and scaled it down to as close to 1 pixel wide corridors as possible without breaking the maze
/// I then converted the black and white image into a txt file with 'B' indicating walls and 'W' indicating open space, and '\n' indicating new rows
/// </summary>
public class MazeMaker : MonoBehaviour
{
    private string path = "Assets/Resources/maze2.txt";

    WallData wallData;
    GroundData stonePathData;

    [SerializeField]
    MapManager mapManager;

    void Start()
    {
        //GenerateMaze(new Vector3Int(0, 0, 0));
    }

    /// <summary>
    /// For use in unity inspector
    /// </summary>
    [ContextMenu("generateMaze")]
    private void GenerateMaze()
    {
        print("generating maze");
        GenerateMaze(new Vector3Int(0, 0, 0));
    }

    /// <summary>
    /// Generates the maze starting at a position
    /// </summary>
    /// <param name="startPosition">Bottom left corner</param>
    public void GenerateMaze(Vector3Int startPosition)
    {
        byte[] bytes = File.ReadAllBytes(path);

        int row = 0;
        int column = 0;

        foreach (byte b in bytes)
        {
            Vector3Int position = startPosition + new Vector3Int(column, row, 0);
            if (b == 'W')
            {
                mapManager.SetTile(position, stonePathData);
                column++;
            }
            else if (b == 'B')
            {
                //  Build floor under wall
                mapManager.SetTile(position, stonePathData);
                //  Build wall
                mapManager.SetTile(position, wallData);
                column++;
            }
            else if (b == '\n')
            {
                row++;
                column = 0;
            }
            else
            {
                Debug.Log("Invalid char: " + b);
            }
        }
    }
}
