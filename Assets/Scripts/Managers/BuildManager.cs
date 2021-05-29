using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BuildManager : IInitializable, IDisposable {

    #region properties
    public BuildMode CurrentBuildMode { get; private set; } = BuildMode.None;
    #endregion

    #region fields
    IMapManager mapManager;
    GameManager gameManager;
    GUIController guiController;

    private Vector3Int lastHoveredPosition = Vector3Int.down;
    private StructureData currentlySelectedStructure;
    private bool lastSelectedStructureWasTower = false;
    private List<GameObject> instantiatedTowers;
    private Vector3Int tilemapOffset;
    private LineRenderer line;
    #endregion

    public enum BuildMode {
        Build,
        Demolish,
        None
    }

    public BuildManager(IMapManager mapManager, GameManager gameManager, GUIController guiController) {
        Debug.Log("build manager constructor");
        this.mapManager = mapManager;
        this.gameManager = gameManager;
        this.guiController = guiController;
    }

    public void Initialize() {
        instantiatedTowers = new List<GameObject>();
        line = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
        MouseManager.OnHoveredNewTile += HandleNewTileHovered;
    }

    public void Dispose() {
        MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
    }

    /// <summary>
    /// Attempts to build a structure at a position
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    private void AttemptBuildStructure(StructureData structure, Vector3Int position) {
        string errorMessage = CanBuildStructure(structure, position);
        if (errorMessage == string.Empty) {
            BuildStructure(structure, position);
            gameManager.SpendGold(structure.Cost);
            guiController.SpawnFloatingText(Camera.main.ScreenToWorldPoint(Input.mousePosition), string.Format("Spent {0}g", structure.Cost), Color.yellow);
        }
        else {
            guiController.SpawnFloatingText(Camera.main.ScreenToWorldPoint(Input.mousePosition), errorMessage, Color.red);
        }
    }

    /// <summary>
    /// Validates structure build conditions
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    /// <returns>Error message or empty string if conditions are valid</returns>
    private string CanBuildStructure (StructureData structure, Vector3Int position) {
        if (CanBuildOverTile(position) == false) {
            return "You can't build there";
        }
        if (gameManager.CanAfford(structure.Cost) == false) {
            return "You can't afford " + structure.Cost  + "g" ;
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
    /// Build a structure over a ground tile
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    private void BuildStructure(StructureData structure, Vector3Int position) {
        if (structure.GetType() == typeof(TowerData)) {
            InstantiateTower((TowerData)structure, position);
            mapManager.SetTile(position, structure);
        }
        else if (structure.GetType() == typeof(WallData)) {
            mapManager.SetTile(position, structure);
        }
        else {
            throw new System.Exception("Structure type " + structure.GetType() + " not implemented");
        }
    }

    /// <summary>
    /// "Highlights" a tower game object at a tilemap position
    /// </summary>
    /// <param name="position">Tilemap position (without offset)</param>
    /// <param name="color">Color to change tint to</param>
    private void ChangeTowerTint(Vector3Int position, Color color) {
        foreach (GameObject tower in instantiatedTowers) {
            if (tower.transform.position == position + tilemapOffset) {
                tower.GetComponentsInChildren<SpriteRenderer>()[0].color = color;
                tower.GetComponentsInChildren<SpriteRenderer>()[1].color = color;
                break;
            }
        }
    }

    /// <summary>
    /// Instantiates and initializes a tower game object and adds it to list
    /// </summary>
    /// <param name="towerData"></param>
    /// <param name="position"></param>
    private void InstantiateTower(TowerData towerData, Vector3Int position) {
        GameObject tower = GameObject.Instantiate(towerData.TowerPrefab, position + tilemapOffset, new Quaternion(0, 0, 0, 0));
        instantiatedTowers.Add(tower);
    }

    /// <summary>
    /// Responds to a complete mouse click
    /// If in build mode, it will attempt to build a structure
    /// 
    /// </summary>
    private void HandleMouseUp() {
        if (CurrentBuildMode != BuildMode.None && gameManager.GameEnded == false &&
            EventSystem.current.IsPointerOverGameObject() == false) {
            Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mouseposition.z = 0;

            if (CurrentBuildMode == BuildMode.Build) {
                if (currentlySelectedStructure.GetType() == typeof(TowerData)) {
                    if (IsTowerAdjacent(mouseposition) == true) {
                        guiController.SpawnFloatingTextAtCursor("You cannot build towers next to eachother", Color.red);
                    }
                    else {
                        BuildStructure(currentlySelectedStructure, mouseposition);
                    }
                }
                else if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, mouseposition) == false) {
                    BuildStructure(currentlySelectedStructure, mouseposition);
                }
            }
            else if (CurrentBuildMode == BuildMode.Demolish) {
                if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, mouseposition)) {
                    DemolishStructure(mouseposition);
                }
            }
            else {
                throw new System.Exception("This build mode is not implemented");
            }
        }
    }

    /// <summary>
    /// Checks if there is a tower within a 3x3 radius of position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsTowerAdjacent(Vector3Int position) {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                Vector3Int neighbour = position + new Vector3Int(x, y, 0);
                if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, neighbour)) {
                    TileData tileData = mapManager.GetTileData(IMapManager.Layer.StructureLayer, neighbour);

                    if (tileData.GetType() == typeof(TowerData)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    #region Hover/Unhover logic

    /// <summary>
    /// Handles logic when a new tile is hovered
    /// </summary>
    /// <param name="tileCoords"></param>
    private void HandleNewTileHovered(Vector3Int tileCoords) {
        if (gameManager.GameEnded == true || 
            CurrentBuildMode == BuildMode.None || 
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

        if (currentlySelectedStructure.GetType() == typeof(TowerData)) {
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
        if (CurrentBuildMode == BuildMode.Build) {
            BuildModeHoverLogic(position);
        }
        else if (CurrentBuildMode == BuildMode.Demolish) {
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
                ChangeTowerTint(position, Color.white);
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
        if (IsTowerAdjacent(position) || CanBuildOverTile(position) == false) {
            HoverTowerGrid(position, Color.red);
        }
        //  Otherwise highligh yellow/green
        else {
            HoverTowerGrid(position, Color.green);
            RenderRadius(position + tilemapOffset, ((TowerData)currentlySelectedStructure).Range);
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
        if (currentlySelectedStructure.GetType() == typeof(TowerData)) {
            BuildTowerHoverLogic(position);
        }
        else {
            if (CanBuildOverTile(position) == true) {
                mapManager.HighlightTile(IMapManager.Layer.GroundLayer, position, Color.green);
            }
            else {
                mapManager.HighlightTile(IMapManager.Layer.StructureLayer, position, Color.red);
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
                ChangeTowerTint(position, Color.green);
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
    private void PauseHighlighting() {
        if (lastSelectedStructureWasTower) {
            UnhoverTowerGrid(lastHoveredPosition);
        }
        else {
            UnhoverTile(lastHoveredPosition);
        }
        lastHoveredPosition = Vector3Int.down;
    }
    #endregion


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

    /// <summary>
    /// Demolishes a structure
    /// </summary>
    /// <param name="position"></param>
    public void DemolishStructure(Vector3Int position) {
        TileData structure = mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);
        if (structure.GetType() == typeof(TowerData)) {
            //  Find and remove tower at this position
            foreach (GameObject tower in instantiatedTowers) {
                if (tower.transform.position == position + tilemapOffset) {
                    TowerData towerData = tower.GetComponent<Tower>().TowerData;
                    int sellValue = Mathf.RoundToInt(towerData.Cost * 0.66f);
                    gameManager.GainGold(sellValue);
                    guiController.SpawnFloatingTextInCenter("Sold for " + sellValue + " gold", Color.yellow);
                    instantiatedTowers.Remove(tower);
                    GameObject.Destroy(tower);
                    mapManager.RemoveTile(IMapManager.Layer.StructureLayer, position);
                    break;
                }
            }
        }
        else if (structure.GetType() == typeof(WallData)) {
            mapManager.RemoveTile(IMapManager.Layer.StructureLayer, position);
        }
        else {
            throw new System.Exception("Stucture type " + structure.GetType() + " not implemented");
        }
    }

    /// <summary>
    /// Enters build mode
    /// </summary>
    /// <param name="selectedStructure"></param>
    public void EnterBuildMode(StructureData selectedStructure) {
        currentlySelectedStructure = selectedStructure;
        lastHoveredPosition = Vector3Int.zero;
        CurrentBuildMode = BuildMode.Build;
        Debug.Log("Entered buildmode for structure: " + selectedStructure.name);
    }

    /// <summary>
    /// Enters demolish mode
    /// </summary>
    public void EnterDemolishMode() {
        lastHoveredPosition = Vector3Int.zero;
        CurrentBuildMode = BuildMode.Demolish;
        Debug.Log("Entered demolish mode");
    }

    /// <summary>
    /// Exits build/demolish mode
    /// </summary>
    public void ExitBuildMode() {
        currentlySelectedStructure = null;
        CurrentBuildMode = BuildMode.None;
        PauseHighlighting();
        Debug.Log("Exited build mode");
    }
}