//  https://www.youtube.com/watch?v=XIqtZnqutGg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundLayer;
    [SerializeField]
    private Tilemap decoreLayer;
    [SerializeField]
    private Tilemap structureLayer;

    [SerializeField]
    private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;

    public enum Layers
    {
        GroundLayer,
        DecoreLayer,
        StructureLayer
    }

    private void Awake()
    {
        //  Link each tile type to their respective shared data attributes
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileDatas)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public void Start()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TileBase tile;
            for (int i = 0; i <= 2; i++)
            {
                tile = SelectTile(i, mouseposition);
                if (tile != null)
                {
                    print(string.Format("layer {0}:{1}", i, tile.name));
                }
                else
                {
                    print(string.Format("Tile {0} is null", i));
                }
            }

            tile = SelectTile((int)Layers.GroundLayer, mouseposition);
            float walkspeed = dataFromTiles[tile].walkSpeed;
            print(string.Format("walk speed on: {0} is {1}", tile, walkspeed));
        }
    }

    /// <summary>
    /// Select a tile from a specified layer
    /// </summary>
    /// <param name="layer">Layer 0 = ground, 1 = decore, 2 = structure</param>
    /// <param name="position">A world point</param>
    /// <returns></returns>
    private TileBase SelectTile(int layer, Vector2 position)
    {
        Tilemap selectLayer;

        switch (layer)
        {
            case 0:
                selectLayer = groundLayer;
                break;
            case 1:
                selectLayer = decoreLayer;
                break;
            case 2:
                selectLayer = structureLayer;
                break;
            default:
                throw new ArgumentException("Layer must be 0, 1, or 2");
        }

        Vector3Int gridPosition = selectLayer.WorldToCell(position);
        TileBase tile = selectLayer.GetTile(gridPosition);
        return tile;
    }

    public float GetTileWalkSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = groundLayer.WorldToCell(worldPosition);
        TileBase tile = groundLayer.GetTile(gridPosition);

        if (tile == null)
        {
            return 1f;
        }

        float walkSpeed = dataFromTiles[tile].walkSpeed;
        return walkSpeed;
    }
}
