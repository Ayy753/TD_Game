//  https://www.youtube.com/watch?v=XIqtZnqutGg

using System;
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

    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (TileData tileData in tileDatas)
        {
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
    /// Sets a tile based on a tileData object
    /// </summary>
    /// <param name="position">Tile position</param>
    /// <param name="tileData">TileData object</param>
    public void SetTile(Vector3Int position, TileData tileData)
    {
        GetLayer(tileData.Layer).SetTile(position, tileData.TileBase);
        if (tileData.Layer == Layer.StructureLayer)
        {
            OnStructureChanged.Invoke();
        }
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

        if (layer == Layer.StructureLayer)
        {
            //  in case this tile was highlighted
            UnhighlightTile(layer, position);
            OnStructureChanged.Invoke();
        }
    }

    /// <summary>
    /// Highlights a tile
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <param name="color"></param>
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

    /// <summary>
    /// Reverts tile to previous highlight color
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    public void ReverseHighlight(Layer layer, Vector3Int position)
    {
        foreach (HighlightedTile tile in highlightedTiles)
        {
            if (layer == tile.Layer && position == tile.Position)
            {
                HighlightTile(layer, position, tile.PreviousColor);
                break;
            }
        }
    }

    /// <summary>
    /// Removes tile highlight
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    public void UnhighlightTile(Layer layer, Vector3Int position)
    {
        foreach (HighlightedTile tile in highlightedTiles)
        {
            if (layer == tile.Layer && position == tile.Position)
            {
                highlightedTiles.Remove(tile);
                GetLayer(layer).SetColor(position, Color.white);
                break;
            }
        }
    }

    /// <summary>
    /// Removes the tinting from all tiles
    /// </summary>
    public void UnhighlightAll()
    {
        while (highlightedTiles.Count > 0)
        {
            UnhighlightTile(highlightedTiles[0].Layer, highlightedTiles[0].Position);
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
