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

    //  Build mode stuff
    private bool buildModeActive = false;
    private TintedTile lastTileHovered;

    //  Stores all the tiles that have been tinted
    private List<TintedTile> tintedtiles;

    /// <summary>
    /// TileMap layer
    /// </summary>
    public enum Layer
    {
        GroundLayer,
        DecoreLayer,
        StructureLayer
    }

    /// <summary>
    /// Tiles belonging to the ground layer
    /// </summary>
    public enum GroundTile
    {
        StonePath,
        Grass
    }

    /// <summary>
    /// Tile belonging to the structure layer
    /// </summary>
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
        tintedtiles = new List<TintedTile>();
    }

    private void Update()
    {
        if (buildModeActive == true)
        {
            Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mouseposition.z = 0;

            if (lastTileHovered != null && lastTileHovered.Position != mouseposition)
            {
                UntintTile(lastTileHovered);
            }

            lastTileHovered = HoverBuildTile(mouseposition);
        }
    }

    public void EnterBuildMode()
    {
        buildModeActive = true;
    }

    public void ExitBuildMode() 
    {
        buildModeActive = false;
    }

    /// <summary>
    /// A class representing a tinted tile on the tilemap (because working with tuples is a pain)
    /// </summary>
    private class TintedTile
    {
        public Layer Layer { get; private set; }
        public Vector3Int Position { get; private set; }

        public TintedTile(Layer layer, Vector3Int position)
        {
            Layer = layer;
            Position = position;
        }
    }

    /// <summary>
    /// Select a tile from a specified layer
    /// </summary>
    /// <param name="layer"></param>
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
    /// <param name="position">A point TileMap point</param>
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

    /// <summary>
    /// Get the TileBase object occupying this position on the ground layer
    /// </summary>
    /// <param name="groundTile"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get the TileBase object occupying this position on the structure layer
    /// </summary>
    /// <param name="structureTile"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Set a ground tile
    /// </summary>
    /// <param name="position"></param>
    /// <param name="groundTile"></param>
    public void SetTile(Vector3Int position, GroundTile groundTile)
    {
        Tilemap tilemap = GetLayer(Layer.GroundLayer);
        TileBase tileBase = GetTileBase(groundTile);
        tilemap.SetTile(position, tileBase);
    }

    /// <summary>
    /// Set a structure tile
    /// </summary>
    /// <param name="position"></param>
    /// <param name="structureTile"></param>
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

        //  Check if there is a tile at this position
        if (tileMap.HasTile(position) == true)
        {
            tileMap.SetTileFlags(position, TileFlags.None);
            tileMap.SetColor(position, color);

            //  Keep track of this tint for later undoing
            tintedtiles.Add(new TintedTile(layer, position));
        }
        else
        {
            Debug.Log("There is no tile at position " + position);
        }
    }

    /// <summary>
    /// Removes the tint from a tile
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    private void UntintTile(TintedTile tintedTile)
    {
        Tilemap tileMap = GetLayer(tintedTile.Layer);

        //Debug.Log("Removing tint from tile at   " + position);
        tileMap.SetColor(tintedTile.Position, Color.white);
        tintedtiles.Remove(tintedTile);
    }

    /// <summary>
    /// If there is a structure at this position, highlight it red
    /// If not, highlight it green
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private TintedTile HoverBuildTile(Vector3Int position)
    {
        TintedTile hoveredTile = null;
        Tilemap structureLayer = GetLayer(Layer.StructureLayer);
        Tilemap groundLayer = GetLayer(Layer.GroundLayer);

        if (structureLayer.HasTile(position))
        {
            structureLayer.SetTileFlags(position, TileFlags.None);
            structureLayer.SetColor(position, Color.red);
            hoveredTile = new TintedTile(Layer.StructureLayer, position);
        }
        else if (groundLayer.HasTile(position))
        {
            groundLayer.SetTileFlags(position, TileFlags.None);
            groundLayer.SetColor(position, Color.green);
            hoveredTile = new TintedTile(Layer.GroundLayer, position);
        }
        
        if (hoveredTile != null)
        {
            tintedtiles.Add(hoveredTile);
        }
        return hoveredTile;
    }

    /// <summary>
    /// Removes the tinting from all tiles
    /// </summary>
    public void ClearAllTints()
    {
        Debug.Log("number of tiles tinted before clearing all tiles:" + tintedtiles.Count);

        while (tintedtiles.Count > 0)
        {
            UntintTile(tintedtiles[0]);
        }

        print("Items in tintedtiles after clearing all tiles: " + tintedtiles.Count);
    }

    /// <summary>
    /// Get the speed attribute of the ground tile occupying a world position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public float GetTileWalkSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = groundLayer.WorldToCell(worldPosition);
        TileBase tile = groundLayer.GetTile(gridPosition);

        if (tile == null)
            return 1f;

        float walkSpeed = dataFromTiles[tile].walkSpeed;
        return walkSpeed;
    }
    
    /// <summary>
    /// Get tile attributes describing a type of tile
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
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
