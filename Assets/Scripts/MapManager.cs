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

    private void Start()
    {
        Debug.Log("Map manager loaded");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TileBase tile;
            for (int i = 0; i <= 2; i++)
            {
                tile = SelectTile((Layers)i, mouseposition);
                if (tile != null)
                    print(string.Format("layer {0}:{1}", i, tile.name));
                else
                    print(string.Format("Tile {0} is null", i));
            }

            tile = SelectTile(Layers.GroundLayer, mouseposition);
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
    public TileBase SelectTile(Layers layer, Vector2 position)
    {
        Tilemap selectLayer = GetLayer(layer);
        Vector3Int gridPosition = selectLayer.WorldToCell(position);
        return selectLayer.GetTile(gridPosition);
    }

    /// <summary>
    /// Select a tile from a specified layer
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public TileBase SelectTile(Layers layer, Vector3Int position)
    {
        Tilemap selectLayer = GetLayer(layer);
        return selectLayer.GetTile(position);
    }

    /// <summary>
    /// Does a coordinate in specified layer contain a tile?
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool ContainsTileAt(Layers layer, Vector3Int position)
    {
        Tilemap selectLayer = GetLayer(layer);
        if (selectLayer.GetTile(position) != null)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Get TileMap corrisponding to Layers enum
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    private Tilemap GetLayer(Layers layer)
    {
        Tilemap selectLayer = groundLayer;
        switch (layer)
        {
            case Layers.GroundLayer:
                return groundLayer;
            case Layers.DecoreLayer:
                return decoreLayer;
            case Layers.StructureLayer:
                return structureLayer;
            default:
                return null;
        }
    }

    /// <summary>
    /// Tints a tile 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public void TintTile(Layers layer, Vector3Int position, Color color)
    {
        Tilemap tileMap = GetLayer(layer);
        tileMap.SetTileFlags(position, TileFlags.None);
        tileMap.SetColor(position, color);
    }

    public float GetTileWalkSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = groundLayer.WorldToCell(worldPosition);
        TileBase tile = groundLayer.GetTile(gridPosition);

        if (tile == null)
            return 1f;

        float walkSpeed = dataFromTiles[tile].walkSpeed;
        return walkSpeed;
    }
    
    public TileData GetTileData(TileBase tile)
    {
        return dataFromTiles[tile];
    }

    /// <summary>
    /// Highlights the tiles in an array on the ground layer
    /// </summary>
    /// <param name="path"></param>
    public void HighlightPath(List<Vector3Int> path, Color color)
    {
        foreach (Vector3Int tile in path)
        {
            TintTile(Layers.GroundLayer, tile, color);
        }
    }
}
