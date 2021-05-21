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
    public void SetTile(TileData tileData, Vector3Int position);
}
