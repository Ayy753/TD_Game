namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using System;
    using UnityEngine;
    using Zenject;

    public class HoverManager : IInitializable, IDisposable {
        private IMapManager mapManager;
        private IBuildValidator hoverValidator;
        private BuildManager buildManager;
        private IWaveManager waveManager;

        private Vector3Int lastHoveredPosition = Vector3Int.down;
        private bool lastSelectedStructureWasTower = false;
        private StructureData structureData;
        private BuildMode buildMode = BuildMode.None;

        public HoverManager(IMapManager mapManager, IBuildValidator hoverValidator, BuildManager buildManager, IWaveManager waveManager) {
            this.mapManager = mapManager;
            this.hoverValidator = hoverValidator;
            this.buildManager = buildManager;
            this.waveManager = waveManager;
        }

        public void Initialize() {
            MouseManager.OnHoveredNewTile += HandleNewTileHovered;
            MouseManager.OnRightMouseUp += PauseHighlighting;
            waveManager.OnWaveStateChanged += HandleWaveStateChanged;
        }

        public void Dispose() {
            MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
            MouseManager.OnRightMouseUp -= PauseHighlighting;
            waveManager.OnWaveStateChanged -= HandleWaveStateChanged;
        }

        private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
            if (arg.NewState == WaveState.WaveInProgress) {
                UnhoverLastTile();
            }
        }

        /// <summary>
        /// Handles logic when a new tile is hovered
        /// </summary>
        /// <param name="position"></param>
        public void HandleNewTileHovered(Vector3Int tileCoords) {
            buildMode = buildManager.CurrentBuildMode;
            structureData = buildManager.CurrentlySelectedStructure;

            if (lastHoveredPosition != Vector3Int.down) {
                UnhoverLastTile();
            }
            if (buildMode != BuildMode.None) {
                lastHoveredPosition = tileCoords;
                HoverTile(tileCoords);
            }
        }

        private void UnhoverLastTile() {
            if (lastSelectedStructureWasTower == true) {
                UnhoverTowerGrid(lastHoveredPosition);
            }
            else {
                mapManager.UnhighlightTopTile(lastHoveredPosition);
            }
        }

        /// <summary>
        /// Highlights the tile being hovered over while in build
        /// or demolish mode
        /// </summary>
        /// <param name="position">Mouse cursor position</param>
        private void HoverTile(Vector3Int position) {
            Color highlightColor;

            if (buildMode == BuildMode.Build) {
                if (hoverValidator.CanBuildStructureOverPosition(position, structureData))
                    highlightColor = Color.green;
                else
                    highlightColor = Color.red;

                if (structureData.GetType() == typeof(TowerData)) {
                    lastSelectedStructureWasTower = true;
                    HoverTowerGrid(position, highlightColor);
                }
                else {
                    lastSelectedStructureWasTower = false;
                    mapManager.HighlightTopTile(position, highlightColor);
                }
            }
            else if (buildMode == BuildMode.Demolish) {
                if (hoverValidator.IsStructurePresentAndDemolishable(position) == true)
                    highlightColor = Color.green;
                else
                    highlightColor = Color.red;
                mapManager.HighlightTopTile(position, highlightColor);
            }
        }

        private void HoverGrid(Vector3Int start, Vector3Int end, Color color) {
            for (int x = start.x; x <= end.x; x++) {
                for (int y = start.y; y <= end.y; y++) {
                    mapManager.HighlightTopTile(new Vector3Int(x, y, 0), color);
                }
            }
        }

        private void UnHoverGrid(Vector3Int start, Vector3Int end) {
            for (int x = start.x; x <= end.x; x++) {
                for (int y = start.y; y <= end.y; y++) {
                    mapManager.HighlightTile(MapLayer.GroundLayer, new Vector3Int(x, y, 0), Color.white);
                    mapManager.UnhighlightTopTile(new Vector3Int(x, y, 0));
                }
            }
        }

        /// <summary>
        /// Hovers a 3x3 grid around tower center
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        private void HoverTowerGrid(Vector3Int position, Color color) {
            Vector3Int bottomLeft = position + new Vector3Int(-1, -1, 0);
            Vector3Int topRight = position + new Vector3Int(1, 1, 0);
            HoverGrid(bottomLeft, topRight, color);

            //  If center contains structure, color it red
            if (mapManager.ContainsTileAt(MapLayer.StructureLayer, position)) {
                mapManager.HighlightTile(MapLayer.StructureLayer, position, Color.red);
            }
        }

        /// <summary>
        /// Unhovers a 3x3 grid around tower center
        /// </summary>
        /// <param name="position"></param>
        private void UnhoverTowerGrid(Vector3Int position) {
            Vector3Int bottomLeft = position + new Vector3Int(-1, -1, 0);
            Vector3Int topRight = position + new Vector3Int(1, 1, 0);
            UnHoverGrid(bottomLeft, topRight);

            //  If center contains structure, remove tint
            if (mapManager.ContainsTileAt(MapLayer.StructureLayer, position)) {
                mapManager.HighlightTile(MapLayer.StructureLayer, position, Color.white);
            }
        }

        /// <summary>
        /// Used when cursor hovers over GUI elements or empty space
        /// or when user exits build/demolish mode
        /// </summary>
        public void PauseHighlighting() {
            if (lastSelectedStructureWasTower) {
                UnhoverTowerGrid(lastHoveredPosition);
            }
            else {
                UnhoverLastTile();
            }
            lastHoveredPosition = Vector3Int.down;
            buildMode = BuildMode.None;
        }

        public void ChangeTowerTint(Vector3 position, Color color) {
            RaycastHit2D hit = Physics2D.Raycast(position, -Vector2.up);

            //  If a gameobject is hit, and the gameobject hit is a tower
            if (hit.collider != null && hit.collider.GetComponent<Tower>() != null) {
                SpriteRenderer[] sprites = hit.collider.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < sprites.Length; i++) {
                    sprites[i].color = color;
                }
            }
        }
    }
}
