using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class HoverManager : IInitializable, IDisposable {
    private IMapManager mapManager;
    private IBuildValidator hoverValidator;
    private BuildManager buildManager;

    private Vector3Int lastHoveredPosition = Vector3Int.down;
    private bool lastSelectedStructureWasTower = false;
    private StructureData structureData;
    private BuildManager.BuildMode buildMode = BuildManager.BuildMode.None;

    public HoverManager(IMapManager mapManager, IBuildValidator hoverValidator, BuildManager buildManager) {
        this.mapManager = mapManager;
        this.hoverValidator = hoverValidator;
        this.buildManager = buildManager;
    }

    public void Initialize() {
        MouseManager.OnHoveredNewTile += HandleNewTileHovered;
        MouseManager.OnRightMouseUp += PauseHighlighting;
        WaveManager.OnStateChanged += HandleWaveStateChanged;
    }

    public void Dispose() {
        MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
        MouseManager.OnRightMouseUp -= PauseHighlighting;
        WaveManager.OnStateChanged -= HandleWaveStateChanged;
    }

    private void HandleWaveStateChanged(WaveManager.State newState) {
        if (newState == WaveManager.State.WaveInProgress) {
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
        if (buildMode != BuildManager.BuildMode.None) {
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

        if (buildMode == BuildManager.BuildMode.Build) {
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
        else if (buildMode == BuildManager.BuildMode.Demolish) {
            if (hoverValidator.CanDemolishStructure(position) == true)
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
                mapManager.HighlightTile(IMapManager.Layer.GroundLayer, new Vector3Int(x, y, 0), Color.white);
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
        if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position)){
            mapManager.HighlightTile(IMapManager.Layer.StructureLayer, position, Color.red);
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
        if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position)){
            mapManager.HighlightTile(IMapManager.Layer.StructureLayer, position, Color.white);
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
        buildMode = BuildManager.BuildMode.None;
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