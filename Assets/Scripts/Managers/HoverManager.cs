using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class HoverManager : IInitializable, IDisposable {
    private IMapManager mapManager;
    private IBuildValidator hoverValidator;
    private GameManager gameManager;

    private Vector3Int lastHoveredPosition = Vector3Int.down;
    private bool lastSelectedStructureWasTower = false;
    private StructureData structureData;
    private BuildManager.BuildMode buildMode = BuildManager.BuildMode.None;

    public HoverManager(IMapManager mapManager, IBuildValidator hoverValidator, GameManager gameManager) {
        this.mapManager = mapManager;
        this.hoverValidator = hoverValidator;
        this.gameManager = gameManager;
    }

    public void Initialize() {
    }

    public void Dispose() {
    }

    /// <summary>
    /// Handles logic when a new tile is hovered
    /// </summary>
    /// <param name="position"></param>
    public void NewTileHovered(Vector3Int position, BuildManager.BuildMode currentBuildMode, StructureData currentStructureData) {
        buildMode = currentBuildMode;
        structureData = currentStructureData;

        if (lastHoveredPosition != Vector3Int.down) {
            Debug.Log("un hovering last tile");
            UnhoverLastTile();
        }
        if (currentBuildMode != BuildManager.BuildMode.None) {
            lastHoveredPosition = position;
            HoverTile(position);
        }
    }

    private void UnhoverLastTile() {
        if (lastSelectedStructureWasTower == true) {
            UnhoverTowerGrid(lastHoveredPosition);
        }
        else {
            if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, lastHoveredPosition) == true) {
                mapManager.HighlightTile(IMapManager.Layer.StructureLayer, lastHoveredPosition, Color.white);
            }
            else {
                mapManager.HighlightTile(IMapManager.Layer.GroundLayer, lastHoveredPosition, Color.white);
            }
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
            if (hoverValidator.CanBuildOverTile(position, structureData))
                highlightColor = Color.green;
            else
                highlightColor = Color.red;

            if (structureData.GetType() == typeof(TowerData)) {
                lastSelectedStructureWasTower = true;
                ChangeTowerTint(position, highlightColor);
                HoverTowerGrid(position, highlightColor);
            }
            else {
                lastSelectedStructureWasTower = false;
                if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position)) 
                    mapManager.HighlightTile(IMapManager.Layer.StructureLayer, position, highlightColor);
                else 
                    mapManager.HighlightTile(IMapManager.Layer.GroundLayer, position, highlightColor);
            }
        }
        else if (buildMode == BuildManager.BuildMode.Demolish) {
            if (hoverValidator.CanDemolishStructure(position) == true)
                highlightColor = Color.green;
            else
                highlightColor = Color.red;

            if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position)) 
                mapManager.HighlightTile(IMapManager.Layer.StructureLayer, position, highlightColor);
            else
                mapManager.HighlightTile(IMapManager.Layer.GroundLayer, position, highlightColor);
        }
    }

    private void UnhoverTile(Vector3Int position) {
        if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position)) {
            TileData tile = mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);

            if (tile.GetType() == typeof(WallData)) {
                mapManager.HighlightTile(IMapManager.Layer.StructureLayer, position, Color.white);
            }
            else if (tile.GetType() == typeof(TowerData)) {
                ChangeTowerTint(position, Color.white);
            }
        }
        else {
            mapManager.HighlightTile(IMapManager.Layer.GroundLayer, position, Color.white);
        }
    }

    private void HoverGrid(Vector3Int start, Vector3Int end, Color color) {
        for (int x = start.x; x <= end.x; x++) {
            for (int y = start.y; y <= end.y; y++) {
                mapManager.HighlightTile(IMapManager.Layer.GroundLayer, new Vector3Int(x, y, 0), color);
            }
        }
    }

    private void UnHoverGrid(Vector3Int start, Vector3Int end) {
        for (int x = start.x; x <= end.x; x++) {
            for (int y = start.y; y <= end.y; y++) {
                UnhoverTile(new Vector3Int(x, y, 0));
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
    }

    /// <summary>
    /// Unhovers a 3x3 grid around tower center
    /// </summary>
    /// <param name="position"></param>
    private void UnhoverTowerGrid(Vector3Int position) {
        Vector3Int bottomLeft = position + new Vector3Int(-1, -1, 0);
        Vector3Int topRight = position + new Vector3Int(1, 1, 0);
        UnHoverGrid(bottomLeft, topRight);
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
            UnhoverTile(lastHoveredPosition);
        }
        lastHoveredPosition = Vector3Int.down;
        buildMode = BuildManager.BuildMode.None;
    }

    public void ChangeTowerTint(Vector3 position, Color color) {
        RaycastHit2D hit = Physics2D.Raycast(position, -Vector2.up);

        if (hit.collider != null) {
            SpriteRenderer[] sprites = hit.collider.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; i++) {
                sprites[i].color = color;
            }
        }
    }
}