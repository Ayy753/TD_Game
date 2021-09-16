namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using System;
    using UnityEngine;
    using Zenject;

    public class HoverManager : IInitializable, IDisposable {
        private IMapManager mapManager;
        private IBuildValidator buildValidator;
        private BuildManager buildManager;
        private IWaveManager waveManager;

        private Vector3Int lastHoveredPosition = Vector3Int.down;
        private bool lastSelectedStructureWasTower = false;
        private StructureData structureData;
        private BuildMode buildMode = BuildMode.None;

        public HoverManager(IMapManager mapManager, IBuildValidator hoverValidator, BuildManager buildManager, IWaveManager waveManager) {
            this.mapManager = mapManager;
            this.buildValidator = hoverValidator;
            this.buildManager = buildManager;
            this.waveManager = waveManager;
        }

        public void Initialize() {
            MouseManager.OnHoveredNewTile += HandleNewTileHovered;
            MouseManager.OnRightMouseUp += PauseHighlighting;
            waveManager.OnWaveStateChanged += HandleWaveStateChanged;
            BuildManager.OnStructureChanged += BuildManager_OnStructureChanged;
        }


        public void Dispose() {
            MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
            MouseManager.OnRightMouseUp -= PauseHighlighting;
            waveManager.OnWaveStateChanged -= HandleWaveStateChanged;
            BuildManager.OnStructureChanged += BuildManager_OnStructureChanged;
        }

        private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
            if (arg.NewState == WaveState.WaveInProgress) {
                UnhoverLastTile();
            }
        }

        private void BuildManager_OnStructureChanged(object sender, StructureChangedEventArgs e) {
            //  Updates tile highlighting 
            HandleNewTileHovered(e.Position);
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

        private void HoverTile(Vector3Int position) {
            if (buildMode == BuildMode.Build) {
                if (structureData is PlatformData) {
                    HoverPlatformBuild(position);
                }
                else {
                    HoverStructureBuild(position);
                }
            }
            else if (buildMode == BuildMode.Demolish) {
                HoverStructureDemolish(position);
            }
        }

        private void HoverPlatformBuild(Vector3Int position) {
            if (buildValidator.CanBuildPlatformOverPosition(position)) {
                mapManager.HighlightTopTile(position, Color.green);
            }
            else {
                mapManager.HighlightTopTile(position, Color.red);
            }
        }

        private void HoverStructureBuild(Vector3Int position) {
            bool buildable = buildValidator.CanBuildStructureOverPosition(position, structureData);

            if (structureData.GetType() == typeof(TowerData)) {
                lastSelectedStructureWasTower = true;
                HoverTowerGrid(position, buildable);
            }
            else {
                lastSelectedStructureWasTower = false;
                mapManager.HighlightTopTile(position, buildable ? Color.green : Color.red);
            }
        }

        private void HoverStructureDemolish(Vector3Int position) {
            if (buildValidator.IsStructurePresentAndDemolishable(position) || buildValidator.IsPlatformPresentAndDemolishable(position))
                mapManager.HighlightTopTile(position, Color.green);
            else
                mapManager.HighlightTopTile(position, Color.red);
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
        /// Highlights a 3x3 grid around tower center
        /// </summary>
        private void HoverTowerGrid(Vector3Int position, bool validPosition) {
            Vector3Int bottomLeft = position + new Vector3Int(-1, -1, 0);
            Vector3Int topRight = position + new Vector3Int(1, 1, 0);

            HoverGrid(bottomLeft, topRight, Color.yellow);

            if (validPosition) {
                mapManager.HighlightTopTile(position, Color.green);
            }
            else {
                mapManager.HighlightTopTile(position, Color.red);
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
