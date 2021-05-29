using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class MapManager : IMapManager, IInitializable {
    private Tilemap groundLayer;
    private Tilemap decoreLayer;
    private Tilemap structureLayer;
    private Tilemap platformLayer;

    TileData[] tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    //  Stores all the tiles that have been tinted
    private List<HighlightedTile> highlightedTiles;

    public void Initialize() {
        Debug.Log("initaliziing MapManager");

        highlightedTiles = new List<HighlightedTile>();
        dataFromTiles = new Dictionary<TileBase, TileData>();
        tileDatas = Resources.LoadAll<TileData>("ScriptableObjects/TileData");

        groundLayer = GameObject.Find("GroundLayer").GetComponent<Tilemap>();
        decoreLayer = GameObject.Find("DecorationLayer").GetComponent<Tilemap>();
        structureLayer = GameObject.Find("StructureLayer").GetComponent<Tilemap>();
        platformLayer = GameObject.Find("PlatformLayer").GetComponent<Tilemap>();

        if (groundLayer == null || decoreLayer == null || structureLayer == null || platformLayer == null) {
            Debug.LogError("One of the tilemap layers is missing");
        }

        Debug.Log("tiledatas count: " + tileDatas.Length);

        if (tileDatas.Length == 0) {
            Debug.LogError("There are no tiledata scriptable objects in the resource folder");
        }

        for (int i = 0; i < tileDatas.Length; i++) {
            //  Link TileBase objects to TileData 
            //  Since towers share the same tower base we need to ensure they dont get added twice
            if (dataFromTiles.ContainsKey(tileDatas[i].TileBase) != true) {
                //Debug.Log(string.Format("Adding: tile name:{0} tilebase:{1}", tileDatas[i].name, tileDatas[i].TileBase.name));
                dataFromTiles.Add(tileDatas[i].TileBase, tileDatas[i]);
            }
        }
    }

    /// <summary>
    /// Get TileMap corrisponding to Layers enum
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    private Tilemap GetLayer(IMapManager.Layer layer) {
        Tilemap selectLayer = groundLayer;
        switch (layer) {
            case IMapManager.Layer.GroundLayer:
                return groundLayer;
            case IMapManager.Layer.DecoreLayer:
                return decoreLayer;
            case IMapManager.Layer.PlatformLayer:
                return platformLayer;
            case IMapManager.Layer.StructureLayer:
                return structureLayer;
            default:
                Debug.LogError("Layer not found");
                return null;
        }
    }

    /// <summary>
    /// Gets the TileData class associated with the TileBase on this layer and position
    /// </summary>
    /// <param name="layer">Layer to search</param>
    /// <param name="position">Position of tile</param>
    /// <returns></returns>
    public TileData GetTileData(IMapManager.Layer layer, Vector3Int position) {
        //  2D tilemap
        position.z = 0;

        TileBase tile = GetLayer(layer).GetTile(position);
        if (tile != null) {
            return dataFromTiles[tile];
        }

        Debug.Log(string.Format("Did not find tile at layer {0} position {1}", layer, position));
        return null;
    }

    public void SetTile(Vector3Int position, TileData tileData) {
        throw new System.NotImplementedException();
    }

    public void RemoveTile(object structureLayer, Vector3Int position) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets tile cost of a ground tile, or the platform built over it
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float GetTileCost(Vector3Int position) {
        if (ContainsTileAt(IMapManager.Layer.PlatformLayer, position)) {
            return ((PlatformData)GetTileData(IMapManager.Layer.PlatformLayer, position)).WalkCost;
        }
        else {
            return ((GroundData)GetTileData(IMapManager.Layer.GroundLayer, position)).WalkCost;
        }
    }

    /// <summary>
    /// Does a coordinate in specified layer contain a tile?
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool ContainsTileAt(IMapManager.Layer layer, Vector3Int position) {
        if (GetLayer(layer).GetTile(position) != null)
            return true;
        else
            return false;
    }

    #region Tile Highlighting
    public void HighlightTile(IMapManager.Layer layer, Vector3Int position, Color color) {
        Tilemap tileMapLayer = GetLayer(layer);
        Color? previousColor = null;

        //  Check if there is a tile at this position
        if (tileMapLayer.HasTile(position) == true) {
            //  If this tile is already tinted, remove it from the collection and keep track of previous color
            foreach (HighlightedTile tile in highlightedTiles) {
                if (tile.Layer == layer && tile.Position == position) {
                    previousColor = tile.Color;
                    highlightedTiles.Remove(tile);
                    break;
                }
            }
            tileMapLayer.SetTileFlags(position, TileFlags.None);
            tileMapLayer.SetColor(position, color);

            //  Don't store highlighted tile if a color is being removed 
            if (color != Color.white) {
                highlightedTiles.Add(new HighlightedTile(position, layer, color, previousColor));
            }
        }
    }

    public void ReverseHighlight(IMapManager.Layer layer, Vector3Int position) {
        foreach (HighlightedTile tile in highlightedTiles) {
            if (layer == tile.Layer && position == tile.Position) {
                HighlightTile(layer, position, tile.PreviousColor);
                break;
            }
        }
    }

    public void UnhighlightTile(IMapManager.Layer layer, Vector3Int position) {
        foreach (HighlightedTile tile in highlightedTiles) {
            if (layer == tile.Layer && position == tile.Position) {
                highlightedTiles.Remove(tile);
                GetLayer(layer).SetColor(position, Color.white);
                break;
            }
        }
    }

    public void UnhighlightAll() {
        while (highlightedTiles.Count > 0) {
            UnhighlightTile(highlightedTiles[0].Layer, highlightedTiles[0].Position);
        }
    }

    /// <summary>
    /// Highlights the tiles in an array on the ground layer
    /// </summary>
    /// <param name="path"></param>
    public void HighlightPath(List<Vector3Int> path, Color color) {
        foreach (Vector3Int tile in path) {
            HighlightTile(IMapManager.Layer.GroundLayer, tile, color);
        }
    }
    #endregion

    public bool IsGroundSolid(Vector3Int position) {
        if (((GroundData)(GetTileData(IMapManager.Layer.GroundLayer, position))).IsSolid) {
            return true;
        }
        return false;
    }
}

