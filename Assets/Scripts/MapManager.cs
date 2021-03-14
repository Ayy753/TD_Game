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

    //  Save from having to add redundant attributes to hundreds of instances of the same tile
    //  https://gameprogrammingpatterns.com/flyweight.html
    private Dictionary<TileBase, TileData> dataFromTiles;

    //  Caching these tiles so it doesnt need to search each time
    private TileBase wallTile;
    private TileBase grassTile;
    private TileBase pathTile;

    /// <summary>
    /// TileMap layer
    /// </summary>
    public enum Layer
    {
        GroundLayer,
        DecoreLayer,
        StructureLayer
    }

    public enum GroundTile
    {
        StonePath,
        Grass
    }

    public enum StructureTile
    {
        Wall
    }

    private void Awake()
    {
        //  Link each tile type to their respective shared data attributes
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileDatas)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                //  Cache tiles (I dont like this solution will probably change later)
                if (tile.name == "StonePathRandom")
                {
                    pathTile = tile;
                }
                else if (tile.name == "WallRuleTile")
                {
                    wallTile = tile;
                }
                else if (tile.name == "GrassRandom")
                {
                    grassTile = tile;
                }

                //  Actually link tile with attributes
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
                tile = SelectTile((Layer)i, mouseposition);
                if (tile != null)
                    print(string.Format("layer {0}:{1}", i, tile.name));
                else
                    print(string.Format("Tile {0} is null", i));
            }

            tile = SelectTile(Layer.GroundLayer, mouseposition);
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
    public TileBase SelectTile(Layer layer, Vector2 position)
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
    public TileBase SelectTile(Layer layer, Vector3Int position)
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
    public bool ContainsTileAt(Layer layer, Vector3Int position)
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
    private Tilemap GetLayer(Layer layer)
    {
        Tilemap selectLayer = groundLayer;
        switch (layer)
        {
            case Layer.GroundLayer:
                return groundLayer;
            case Layer.DecoreLayer:
                return decoreLayer;
            case Layer.StructureLayer:
                return structureLayer;
            default:
                return null;
        }
    }

    private TileBase GetTileBase(GroundTile groundTile)
    {
        switch (groundTile)
        {
            case GroundTile.StonePath:
                return pathTile;
            case GroundTile.Grass:
                return grassTile;
            default:
                return null;
        }
    }
    private TileBase GetTileBase(StructureTile structureTile)
    {
        switch (structureTile)
        {
            case StructureTile.Wall:
                return wallTile;
            default:
                return null;
        }
    }

    public void SetTile(Vector3Int position, GroundTile groundTile)
    {
        Tilemap tilemap = GetLayer(Layer.GroundLayer);
        TileBase tileBase = GetTileBase(groundTile);
        tilemap.SetTile(position, tileBase);
    }

    public void SetTile(Vector3Int position, StructureTile structureTile)
    {
        Tilemap tilemap = GetLayer(Layer.StructureLayer);
        TileBase tileBase = GetTileBase(structureTile);
        tilemap.SetTile(position, tileBase);
    }

    /// <summary>
    /// Tints a tile 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public void TintTile(Layer layer, Vector3Int position, Color color)
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
            TintTile(Layer.GroundLayer, tile, color);
        }
    }
}
