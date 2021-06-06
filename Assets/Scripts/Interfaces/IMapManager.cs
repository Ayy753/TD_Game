using System.Collections.Generic;
using UnityEngine;

public interface IMapManager
{
    public enum Layer
    {
        GroundLayer,
        DecoreLayer,
        StructureLayer,
        PlatformLayer
    }
    public TileData GetTileData(Layer layer, Vector3Int position);
    public void SetTile(Vector3Int position, TileData tileData);
    float GetTileCost(Vector3Int neighbourCoordinate);
    bool ContainsTileAt(Layer structureLayer, Vector3Int position);


    public void HighlightTile(IMapManager.Layer layer, Vector3Int position, Color color);

    /// <summary>
    /// Removes tile highlight
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    public void UnhighlightTile(IMapManager.Layer layer, Vector3Int position);

    /// <summary>
    /// Removes the tinting from all tiles
    /// </summary>
    public void UnhighlightAll();

    /// <summary>
    /// Highlights the tiles in an array on the ground layer
    /// </summary>
    /// <param name="path"></param>
    public void HighlightPath(List<Vector3Int> path, Color color);

    bool IsGroundSolid(Vector3Int position);
    void RemoveTile(Layer layer, Vector3Int position);
}
