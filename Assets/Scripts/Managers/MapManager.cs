namespace DefaultNamespace.TilemapSystem {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;
    using Zenject;

    public class MapManager : MonoBehaviour, IMapManager, IInitializable {
        private Dictionary<TileBase, TileData> dataFromTiles;
        private Dictionary<IMapManager.Layer, Tilemap> layerToTilemap;

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

            layerToTilemap = new Dictionary<IMapManager.Layer, Tilemap>() {
                {IMapManager.Layer.GroundLayer, groundLayer },
                {IMapManager.Layer.DecoreLayer, decoreLayer },
                {IMapManager.Layer.PlatformLayer, platformLayer },
                {IMapManager.Layer.StructureLayer, structureLayer },
                {IMapManager.Layer.Level1, level1 },
                {IMapManager.Layer.Level2, level2 },
                {IMapManager.Layer.Level3, level3 },
                {IMapManager.Layer.Level4, level4 }
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
                if (dataFromTiles.ContainsKey(tileDatas[i].TileBase) != true) {
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
            return layerToTilemap[layer];
        }

        /// <summary>
        /// Gets the TileData class associated with the TileBase on this layer and position
        /// Note: Tilebase is the dictionary key and there are multiple tower types that share the same towerbase, and the same key can't exist twice...
        /// Therefore only one towerbase key was added in the dictonary and this should not be used to get the tower type at this position.
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
            return null;
        }

        public void SetTile(Vector3Int position, TileData tileData) {
            IMapManager.Layer layer = tileData.Layer;
            GetLayer(layer).SetTile(position, tileData.TileBase);
        }

        public void RemoveTile(IMapManager.Layer layer, Vector3Int position) {
            GetLayer(layer).SetTile(position, null);
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

        public void UnhighlightTile(IMapManager.Layer layer, Vector3Int position) {
            foreach (HighlightedTile tile in highlightedTiles) {
                if (layer == tile.Layer && position == tile.Position) {
                    highlightedTiles.Remove(tile);
                    GetLayer(layer).SetColor(position, Color.white);
                    break;
                }
            }
        }

        public void HighlightTopTile(Vector3Int position, Color color) {
            for (int i = (int)IMapManager.Layer.Max - 1; i >= 0; i--) {
                if (ContainsTileAt((IMapManager.Layer)i, position)) {
                    HighlightTile((IMapManager.Layer)i, position, color);
                }
            }
        }

        public void UnhighlightTopTile(Vector3Int position) {
            for (int i = (int)IMapManager.Layer.Max - 1; i >= 0; i--) {
                if (ContainsTileAt((IMapManager.Layer)i, position)) {
                    UnhighlightTile((IMapManager.Layer)i, position);
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

        public Vector3Int[] GetTilePositionsOnLayer(IMapManager.Layer layer) {
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
    }
}