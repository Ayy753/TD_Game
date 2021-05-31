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
    HoverManager hoverManager;

    public StructureData currentlySelectedStructure;
    private List<GameObject> instantiatedTowers;
    private Vector3Int tilemapOffset;
    #endregion

    public enum BuildMode {
        Build,
        Demolish,
        None
    }

    public BuildManager(IMapManager mapManager, GameManager gameManager, GUIController guiController, HoverManager hoverManager) {
        Debug.Log("build manager constructor");
        this.mapManager = mapManager;
        this.gameManager = gameManager;
        this.guiController = guiController;
        this.hoverManager = hoverManager;
    }

    public void Initialize() {
        instantiatedTowers = new List<GameObject>();
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
        string errorMessage = hoverManager.CanBuildStructure(structure, position);
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
    /// Handles logic when a new tile is hovered
    /// </summary>
    /// <param name="tileCoords"></param>
    private void HandleNewTileHovered(Vector3Int tileCoords) {
        hoverManager.NewTileHovered(tileCoords, CurrentBuildMode, currentlySelectedStructure);
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
    public bool IsTowerAdjacent(Vector3Int position) {
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

        CurrentBuildMode = BuildMode.Build;
        Debug.Log("Entered buildmode for structure: " + selectedStructure.name);
    }

    /// <summary>
    /// Enters demolish mode
    /// </summary>
    public void EnterDemolishMode() {

        CurrentBuildMode = BuildMode.Demolish;
        Debug.Log("Entered demolish mode");
    }

    /// <summary>
    /// Exits build/demolish mode
    /// </summary>
    public void ExitBuildMode() {
        currentlySelectedStructure = null;
        CurrentBuildMode = BuildMode.None;
        hoverManager.PauseHighlighting();
        Debug.Log("Exited build mode");
    }

    public GameObject GetTowerAtPosition(Vector3Int position) {
        foreach (GameObject tower in instantiatedTowers) {
            if (tower.transform.position == position + tilemapOffset) {
                return tower;
            }
        }
        return null;
    }
}