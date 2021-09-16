namespace DefaultNamespace.TilemapSystem {

    using System.Collections.Generic;
    using UnityEngine;

    public enum MapLayer {
        GroundLayer,
        Level1,
        Level2,
        Level3,
        Level4,
        PlatformLayer,
        StructureLayer,
        DecoreLayer,
        Max
    }

    public interface IMapManager {
        public TileData GetTileData(MapLayer layer, Vector3Int position);
        public void SetTile(Vector3Int position, TileData tileData);
        float GetTileCost(Vector3Int neighbourCoordinate);
        bool ContainsTileAt(MapLayer structureLayer, Vector3Int position);
        public void HighlightTile(MapLayer layer, Vector3Int position, Color color);
        public void UnhighlightTile(MapLayer layer, Vector3Int position);
        public void HighlightTopTile(Vector3Int position, Color color);
        public void UnhighlightTopTile(Vector3Int position);
        public void UnhighlightAll();
        bool CanGroundOrPlatformBeBuiltOn(Vector3Int position);
        void RemoveTile(MapLayer layer, Vector3Int position);
        public Vector3Int[] GetTilePositionsOnLayer(MapLayer layer);
        TileData GetTopLayerTileData(Vector3Int position, bool ignoreDecor);
    }
}
