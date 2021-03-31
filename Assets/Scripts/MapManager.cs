//  https://www.youtube.com/watch?v=XIqtZnqutGg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

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

    //  Caching these tiles so it doesnt need to search each time
    private TileBase wallTile;
    private TileBase grassTile;
    private TileBase pathTile;
    private TileBase towerTile;

    //  Stores all the tiles that have been tinted
    private List<HighlightedTile> highlightedTiles;

    public delegate void StructureChanged();
    public static event StructureChanged OnStructureChanged;

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
    public enum StructureType
    {
        Wall,
        TowerBase
    }

    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (TileData tileData in tileDatas)
        {
            //  Cache tiles (I dont like this solution will probably change later)
            if (tileData.name == "StonePathRandom")
            {
                pathTile = tileData.TileBase;
            }
            else if (tileData.name == "WallRuleTile")
            {
                wallTile = tileData.TileBase;
            }
            else if (tileData.name == "GrassRandom")
            {
                grassTile = tileData.TileBase;
            }
            else if (tileData.name == "TowerBase")
            {
                towerTile = tileData.TileBase;
            }

            //  Link TileBase objects to TileData 
            //  Since towers share the same tower base we need to ensure they dont get added twice
            if (dataFromTiles.ContainsKey(tileData.TileBase) != true)
            {
                Debug.Log(string.Format("Adding: tile name:{0} tilebase:{1}", tileData.name, tileData.TileBase.name));
                dataFromTiles.Add(tileData.TileBase, tileData);
            }
        }
    }

    private void Start()
    {
        Debug.Log("Map manager loaded");
        highlightedTiles = new List<HighlightedTile>();
    }

    ///// <summary>
    ///// Select a tile from a specified layer
    ///// </summary>
    ///// <param name="layer"></param>
    ///// <param name="position">A world point</param>
    ///// <returns></returns>
    //public TileBase SelectTile(Layer layer, Vector2 position)
    //{
    //    Tilemap selectLayer = GetLayer(layer);
    //    Vector3Int gridPosition = selectLayer.WorldToCell(position);
    //    return selectLayer.GetTile(gridPosition);
    //}

    ///// <summary>
    ///// Select a tile from a specified layer
    ///// </summary>
    ///// <param name="layer"></param>
    ///// <param name="position">A point TileMap point</param>
    ///// <returns></returns>
    //public TileBase SelectTile(Layer layer, Vector3Int position)
    //{
    //    Tilemap selectLayer = GetLayer(layer);
    //    return selectLayer.GetTile(position);
    //}

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
    /// Get the Ground layer TileBase object represented by an enumerator
    /// </summary>
    /// <param name="groundTile"></param>
    /// <returns></returns>
    public TileBase GetTileBase(GroundTile groundTile)
    {
        switch (groundTile)
        {
            case GroundTile.StonePath:
                return pathTile;
            case GroundTile.Grass:
                return grassTile;
            default:
                throw new Exception("Could not find a ground layer TileBase object associated with this enum");
        }
    }

    /// <summary>
    /// Get the Structure layer TileBase object represented by an enumerator
    /// </summary>
    /// <param name="structureTile"></param>
    /// <returns></returns>
    public TileBase GetTileBase(StructureType structureTile)
    {
        switch (structureTile)
        {
            case StructureType.Wall:
                return wallTile;
            case StructureType.TowerBase:
                return towerTile;
            default:
                throw new Exception("Could not find a structure layer TileBase object associated with this enum");
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
    /// Sets a tile based on a tileData object
    /// </summary>
    /// <param name="position">Tile position</param>
    /// <param name="tileData">TileData object</param>
    public void SetTile(Vector3Int position, TileData tileData)
    {
        GetLayer(tileData.Layer).SetTile(position, tileData.TileBase);
    }

    /// <summary>
    /// Set a structure tile
    /// </summary>
    /// <param name="position"></param>
    /// <param name="structureTile"></param>
    public void SetTile(Vector3Int position, StructureType structureTile)
    {
        Tilemap tilemap = GetLayer(Layer.StructureLayer);
        TileBase tileBase = GetTileBase(structureTile);
        tilemap.SetTile(position, tileBase);
    }

    /// <summary>
    /// Removes a tile from a layer at a location
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    public void RemoveTile(Layer layer, Vector3Int position)
    {
        Tilemap tilemap = GetLayer(layer);
        tilemap.SetTile(position, null);
    }

    public void HighlightTile(Layer layer, Vector3Int position, Color color)
    {
        Tilemap tileMapLayer = GetLayer(layer);
        Color? previousColor = null;

        //  Check if there is a tile at this position
        if (tileMapLayer.HasTile(position) == true)
        {
            //  If this tile is already tinted, remove it from the collection and keep track of previous color
            foreach (HighlightedTile tile in highlightedTiles)
            {
                if (tile.Layer == layer && tile.Position == position)
                {
                    previousColor = tile.Color;
                    highlightedTiles.Remove(tile);
                    break;
                }
            }

            tileMapLayer.SetTileFlags(position, TileFlags.None);
            tileMapLayer.SetColor(position, color);

            highlightedTiles.Add(new HighlightedTile(position, layer, color, previousColor));
        }
    }

    public void ReverseHighlight(Layer layer, Vector3Int position)
    {
        foreach (HighlightedTile tile in highlightedTiles)
        {
            if (layer == tile.Layer && position == tile.Position)
            {
                Debug.Log(string.Format("Reversing highlight at {0}, {1}", layer, position));
                HighlightTile(layer, position, tile.PreviousColor);
                break;
            }
        }
    }

    public void UnhighlightTile(Layer layer, Vector3Int position)
    {
        foreach (HighlightedTile tile in highlightedTiles)
        {
            if (layer == tile.Layer && position == tile.Position)
            {
                GetLayer(layer).SetColor(position, Color.white);
                highlightedTiles.Remove(tile);
                break;
            }
        }
    }

    /// <summary>
    /// Removes the tint from a tile
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    private void UntintTile(HighlightedTile tintedTile)
    {
        Tilemap tileMap = GetLayer(tintedTile.Layer);

        //Debug.Log("Removing tint from tile at   " + position);
        tileMap.SetColor(tintedTile.Position, Color.white);
        highlightedTiles.Remove(tintedTile);
    }

    /// <summary>
    /// Removes the tinting from all tiles
    /// </summary>
    public void ClearAllTints()
    {
        //Debug.Log("number of tiles tinted before clearing all tiles:" + tintedtiles.Count);

        while (highlightedTiles.Count > 0)
        {
            UntintTile(highlightedTiles[0]);
        }
    }

    /// <summary>
    /// Gets the TileData class associated with the TileBase on this layer and position
    /// </summary>
    /// <param name="layer">Layer to search</param>
    /// <param name="position">Position of tile</param>
    /// <returns></returns>
    public  TileData GetTileData(Layer layer, Vector3Int position)
    {
        TileBase tile = GetLayer(layer).GetTile(position);
        if (tile != null)
        {
            //Debug.Log("Hovered over: " + tile.name);
            return dataFromTiles[tile];
        }
        return null;
    }

    /// <summary>
    /// Highlights the tiles in an array on the ground layer
    /// </summary>
    /// <param name="path"></param>
    public void HighlightPath(List<Vector3Int> path, Color color)
    {
        foreach (Vector3Int tile in path)
        {
            HighlightTile(Layer.GroundLayer, tile, color);
        }
    }
}
