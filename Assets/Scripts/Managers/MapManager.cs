namespace DefaultNamespace.TilemapSystem {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;
    using Zenject;

    public class MapManager : MonoBehaviour, IMapManager, IInitializable {
        private Dictionary<TileBase, TileData> dataFromTiles;
        private Dictionary<MapLayer, Tilemap> layerToTilemap;

        private readonly List<HighlightedTile> highlightedTiles = new List<HighlightedTile>();

        public void Initialize() {
            Debug.Log("initaliziing MapManager");

            InitializeTilemap();
            InitializeTileData();
        }

        private void InitializeTilemap() {
            Tilemap groundLayer, decoreLayer, structureLayer, platformLayer, level1, level2, level3, level4;

            groundLayer = GameObject.Find("GroundLayer").GetComponent<Tilemap>();
            decoreLayer = GameObject.Find("DecorationLayer").GetComponent<Tilemap>();
            structureLayer = GameObject.Find("StructureLayer").GetComponent<Tilemap>();
            platformLayer = GameObject.Find("PlatformLayer").GetComponent<Tilemap>();

            level1 = GameObject.Find("Level1").GetComponent<Tilemap>();
            level2 = GameObject.Find("Level2").GetComponent<Tilemap>();
            level3 = GameObject.Find("Level3").GetComponent<Tilemap>();
            level4 = GameObject.Find("Level4").GetComponent<Tilemap>();

            layerToTilemap = new Dictionary<MapLayer, Tilemap>() {
                {MapLayer.GroundLayer, groundLayer },
                {MapLayer.DecoreLayer, decoreLayer },
                {MapLayer.PlatformLayer, platformLayer },
                {MapLayer.StructureLayer, structureLayer },
                {MapLayer.Level1, level1 },
                {MapLayer.Level2, level2 },
                {MapLayer.Level3, level3 },
                {MapLayer.Level4, level4 }
            };
        }

        private void InitializeTileData() {
            TileData[] tileDatas = Resources.LoadAll<TileData>("ScriptableObjects/TileData");

            dataFromTiles = new Dictionary<TileBase, TileData>();

            if (tileDatas.Length == 0) {
                Debug.LogError("There are no tiledata scriptable objects in the resource folder");
            }

            for (int i = 0; i < tileDatas.Length; i++) {
                //  Link TileBase objects to TileData 
                //  Since towers share the same tower base we need to ensure they dont get added twice

                Debug.Log(tileDatas[i].Name);

                if (dataFromTiles.ContainsKey(tileDatas[i].TileBase) != true) {
                    dataFromTiles.Add(tileDatas[i].TileBase, tileDatas[i]);
                }
            }
        }
        
        private Tilemap GetLayer(MapLayer layer) {
            return layerToTilemap[layer];
        }

        /// <summary>
        /// Gets the TileData class associated with the TileBase on this layer and position
        /// Note: Tilebase is the dictionary key and there are multiple tower types that share the same towerbase, and the same key can't exist twice...
        /// Therefore only one towerbase key was added in the dictonary and this should not be used to get the tower type at this position.
        /// </summary>
        public TileData GetTileData(MapLayer layer, Vector3Int position) {
            //  2D tilemap
            position.z = 0;

            TileBase tile = GetLayer(layer).GetTile(position);
            if (tile != null) {
                return dataFromTiles[tile];
            }
            return null;
        }

        public void SetTile(Vector3Int position, TileData tileData) {
            MapLayer layer = tileData.Layer;
            GetLayer(layer).SetTile(position, tileData.TileBase);
        }

        public void RemoveTile(MapLayer layer, Vector3Int position) {
            GetLayer(layer).SetTile(position, null);
        }

        public float GetTileCost(Vector3Int position) {
            if (ContainsTileAt(MapLayer.PlatformLayer, position)) {
                return ((PlatformData)GetTileData(MapLayer.PlatformLayer, position)).WalkCost;
            }
            else {
                return ((GroundData)GetTileData(MapLayer.GroundLayer, position)).WalkCost;
            }
        }

        public bool ContainsTileAt(MapLayer layer, Vector3Int position) {
            if (GetLayer(layer).GetTile(position) != null)
                return true;
            else
                return false;
        }

        #region Tile Highlighting
        public void HighlightTile(MapLayer layer, Vector3Int position, Color color) {
            Tilemap tileMapLayer = GetLayer(layer);

            if (tileMapLayer.HasTile(position)) {
                RemoveHighlightedTile(layer, position);
                AddHighlightedTile(layer, position, color);
            }
        }

        private void RemoveHighlightedTile(MapLayer layer, Vector3Int position) {
            foreach (HighlightedTile tile in highlightedTiles) {
                if (tile.Layer == layer && tile.Position == position) {
                    highlightedTiles.Remove(tile);
                    break;
                }
            }
        }

        private void AddHighlightedTile(MapLayer layer, Vector3Int position, Color color) {
            Tilemap tileMapLayer = GetLayer(layer);

            tileMapLayer.SetTileFlags(position, TileFlags.None);
            tileMapLayer.SetColor(position, color);
            highlightedTiles.Add(new HighlightedTile(position, layer, color));
        }

        public void UnhighlightTile(MapLayer layer, Vector3Int position) {
            foreach (HighlightedTile tile in highlightedTiles) {
                if (layer == tile.Layer && position == tile.Position) {
                    highlightedTiles.Remove(tile);
                    GetLayer(layer).SetColor(position, Color.white);
                    break;
                }
            }
        }

        public void HighlightTopTile(Vector3Int position, Color color) {
            MapLayer topLayer = GetTopLayer(position, true);

            if (topLayer != MapLayer.TheVoid) {
                HighlightTile(topLayer, position, color);
            }
        }

        public void UnhighlightTopTile(Vector3Int position) {
            MapLayer topLayer = GetTopLayer(position, true);

            if (topLayer != MapLayer.TheVoid) {
                UnhighlightTile(topLayer, position);
            }
        }

        public void UnhighlightAll() {
            while (highlightedTiles.Count > 0) {
                UnhighlightTile(highlightedTiles[0].Layer, highlightedTiles[0].Position);
            }
        }
        #endregion

        public bool CanGroundOrPlatformBeBuiltOn(Vector3Int position) {
            if (ContainsTileAt(MapLayer.GroundLayer, position)) {
                if (ContainsTileAt(MapLayer.PlatformLayer, position) && ((PlatformData)(GetTileData(MapLayer.PlatformLayer, position))).CanBeBuiltOn) {
                    return true;
                }
                else if (((GroundData)(GetTileData(MapLayer.GroundLayer, position))).IsSolid) {
                    return true;
                }
            }
            return false;
        }

        public Vector3Int[] GetTilePositionsOnLayer(MapLayer layer) {
            Tilemap tilemap = GetLayer(layer);
            List<Vector3Int> tilePositions = new List<Vector3Int>();

            for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++) {
                for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++) {
                    Vector3Int position = new Vector3Int(x, y, (int)tilemap.transform.position.z);
                    if (ContainsTileAt(layer, position)) {
                        tilePositions.Add(position);
                    }
                }
            }
            return tilePositions.ToArray();
        }

        public TileData GetTopLayerTileData(Vector3Int position, bool ignoreDecor) {
            MapLayer topLayer = GetTopLayer(position, ignoreDecor);

            if (topLayer != MapLayer.TheVoid) {
                return GetTileData(topLayer, position);
            }
            else {
                return null;
            }
        }

        private MapLayer GetTopLayer(Vector3Int position, bool ignoreDecor) {
            MapLayer layer;
            int layerCount = (int)MapLayer.Max - 2;

            for (int i = layerCount; i >= 0; i--) {
                layer = (MapLayer)i;
                if ((ignoreDecor && layer != MapLayer.DecoreLayer) && ContainsTileAt(layer, position)) {
                    return layer;
                }
            }
            return MapLayer.TheVoid;
        }
    }
}