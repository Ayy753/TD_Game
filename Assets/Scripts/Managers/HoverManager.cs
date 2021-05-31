using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class HoverManager : IInitializable, IDisposable {
    private IMapManager mapManager;
    private GameManager gameManager;

    private Vector3Int lastHoveredPosition = Vector3Int.down;
    private bool lastSelectedStructureWasTower = false;
    private StructureData structureData;
    private BuildManager.BuildMode buildMode;

    private LineRenderer line;
    private readonly Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);

    public HoverManager(IMapManager mapManager, GameManager gameManager) {
        this.mapManager = mapManager;
        this.gameManager = gameManager;
    }

    public void Initialize() {
        line = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
    }

    public void Dispose() {
    }

    /// <summary>
    /// Handles logic when a new tile is hovered
    /// </summary>
    /// <param name="tileCoords"></param>
    public void NewTileHovered(Vector3Int tileCoords, BuildManager.BuildMode currentBuildMode, StructureData currentStructureData) {
        buildMode = currentBuildMode;
        structureData = currentStructureData;

        if (gameManager.GameEnded == true ||
            currentBuildMode == BuildManager.BuildMode.None ||
            EventSystem.current.IsPointerOverGameObject() == true ||
            mapManager.ContainsTileAt(IMapManager.Layer.GroundLayer, tileCoords) == false) {

            PauseHighlighting();
            return;
        }

        if (lastSelectedStructureWasTower) {
            BuildTowerUnHoverLogic();
        }
        else {
            UnhoverTile(lastHoveredPosition);
        }

        if (currentStructureData.GetType() == typeof(TowerData)) {
            BuildTowerHoverLogic(tileCoords);
        }
        else {
            HoverTile(tileCoords);
        }
    }

    /// <summary>
    /// Highlights the tile being hovered over while in build
    /// or demolish mode
    /// </summary>
    /// <param name="position">Mouse cursor position</param>
    private void HoverTile(Vector3Int position) {
        if (buildMode == BuildManager.BuildMode.Build) {
            BuildModeHoverLogic(position);
        }
        else if (buildMode == BuildManager.BuildMode.Demolish) {
            DemolishModeHoverLogic(position);
        }
        else {
            throw new System.Exception("This build mode is not implemented");
        }

        lastHoveredPosition = position;
    }

    /// <summary>
    /// Unhighlights a tile
    /// </summary>
    /// <param name="position"></param>
    private void UnhoverTile(Vector3Int position) {
        if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position)) {
            TileData tile = mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);

            if (tile.GetType() == typeof(WallData)) {
                mapManager.ReverseHighlight(IMapManager.Layer.StructureLayer, position);
            }
            else if (tile.GetType() == typeof(TowerData)) {
                //ChangeTowerTint(position, Color.white);
            }
            else {
                throw new System.Exception("Structure type not implemented");
            }
        }
        else {
            mapManager.ReverseHighlight(IMapManager.Layer.GroundLayer, position);
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
    /// Highlights 3x3 grid around tower in build mode.
    /// Should only be called if the user is in buildmode, 
    /// has a tower selected, and is hovering over a stable ground tile
    /// </summary>
    /// <param name="position"></param>
    private void BuildTowerHoverLogic(Vector3Int position) {

        //  Highlight grid red if another tower is present in it
        if (/*buildManager.IsTowerAdjacent(position) || */ CanBuildOverTile(position) == false) {
            HoverTowerGrid(position, Color.red);
        }
        //  Otherwise highligh yellow/green
        else {
            HoverTowerGrid(position, Color.green);
            RenderRadius(position + tilemapOffset, ((TowerData)structureData).Range);
        }
        lastHoveredPosition = position;
        lastSelectedStructureWasTower = true;
    }

    private void BuildTowerUnHoverLogic() {
        UnhoverTowerGrid(lastHoveredPosition);

        lastSelectedStructureWasTower = false;
        HideRadius();
    }

    /// <summary>
    /// Handles hover logic while in build mode
    /// </summary>
    /// <param name="position"></param>
    private void BuildModeHoverLogic(Vector3Int position) {
        if (structureData.GetType() == typeof(TowerData)) {
            BuildTowerHoverLogic(position);
        }
        else {
            if (CanBuildOverTile(position) == true) {
                mapManager.HighlightTile(IMapManager.Layer.GroundLayer, position, Color.green);
            }
            else {
                mapManager.HighlightTile(IMapManager.Layer.GroundLayer, position, Color.red);
            }
        }
    }

    /// <summary>
    /// Handles hover logic while in demolish mode
    /// </summary>
    /// <param name="position"></param>
    private void DemolishModeHoverLogic(Vector3Int position) {
        if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position)) {
            StructureData structure = (StructureData)mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);

            if (structure.GetType() == typeof(TowerData)) {
                //ChangeTowerTint(position, Color.green);
            }
            else {
                mapManager.HighlightTile(IMapManager.Layer.StructureLayer, position, Color.green);
            }
        }
        else {
            mapManager.HighlightTile(IMapManager.Layer.GroundLayer, position, Color.red);
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
            UnhoverTile(lastHoveredPosition);
        }
        lastHoveredPosition = Vector3Int.down;
    }

    /// <summary>
    /// Validates structure build conditions
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    /// <returns>Error message or empty string if conditions are valid</returns>
    public string CanBuildStructure(StructureData structure, Vector3Int position) {
        if (CanBuildOverTile(position) == false) {
            return "You can't build there";
        }
        if (gameManager.CanAfford(structure.Cost) == false) {
            return "You can't afford " + structure.Cost + "g";
        }
        return string.Empty;
    }

    private bool CanBuildOverTile(Vector3Int position) {
        if (mapManager.ContainsTileAt(IMapManager.Layer.GroundLayer, position) == false ||
            mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position) == true ||
            mapManager.IsGroundSolid(position) == false) {

            return false;
        }
        return true;
    }

    /// <summary>
    /// Renders a line of a given radius around a point to draw a circle
    /// used to show the radius of things
    /// </summary>
    /// <param name="center">Center of tower</param>
    /// <param name="radius">Attack radius of tower</param>
    public void RenderRadius(Vector3 center, float radius) {
        line.enabled = true;
        float x;
        float y;

        //  Drawing a line around the given position
        for (int i = 0; i < line.positionCount; i++) {
            x = center.x + radius * Mathf.Sin(Mathf.Deg2Rad * (360 / line.positionCount * i));
            y = center.y + radius * Mathf.Cos(Mathf.Deg2Rad * (360 / line.positionCount * i));

            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    /// <summary>
    /// Hides the line
    /// </summary>
    public void HideRadius() {
        line.enabled = false;
    }

    ///// <summary>
    ///// "Highlights" a tower game object at a tilemap position
    ///// </summary>
    ///// <param name="position">Tilemap position (without offset)</param>
    ///// <param name="color">Color to change tint to</param>
    //private void ChangeTowerTint(Vector3Int position, Color color) {
    //    GameObject tower = buildManager.GetTowerAtPosition(position);
    //    if (tower != null) {
    //        tower.GetComponentsInChildren<SpriteRenderer>()[0].color = color;
    //        tower.GetComponentsInChildren<SpriteRenderer>()[1].color = color;
    //    }
    //}
}