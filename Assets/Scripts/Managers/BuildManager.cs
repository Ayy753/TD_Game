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
    IBuildValidator buildValidator;
    IMessageSystem messageSystem;
    IWallet wallet;
    HoverManager hoverManager;
    GameManager gameManager;
    ObjectPool objectPool;
    RadiusRenderer radiusRenderer;

    public StructureData currentlySelectedStructure;
    private Vector3 tilemapOffset = new Vector3( 0.5f, 0.5f, 0);
    private Vector3Int lastPositionHovered;

    public delegate void StructureChanged();
    public event StructureChanged OnStructureChanged;

    #endregion

    public enum BuildMode {
        Build,
        Demolish,
        None
    }

    public BuildManager(IMapManager mapManager, GameManager gameManager, IBuildValidator buildValidator, IMessageSystem messageSystem, IWallet wallet, HoverManager hoverManager, ObjectPool objectPool, RadiusRenderer radiusRenderer) {
        Debug.Log("build manager constructor");
        this.mapManager = mapManager;
        this.gameManager = gameManager;
        this.buildValidator = buildValidator;
        this.messageSystem = messageSystem;
        this.wallet = wallet;
        this.hoverManager = hoverManager;
        this.objectPool = objectPool;
        this.radiusRenderer = radiusRenderer;
    }

    public void Initialize() {
        MouseManager.OnHoveredNewTile += HandleNewTileHovered;
        MouseManager.OnMouseUp += HandleMouseUp;
    }

    public void Dispose() {
        MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
        MouseManager.OnMouseUp -= HandleMouseUp;
    }

    /// <summary>
    /// Attempts to build a structure at a position
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    private void AttemptBuildStructure(StructureData structure, Vector3Int position) {
        
        //  Validate if user can build at this position and can afford structure
        if (buildValidator.CanBuildOverTile(position, structure) == false) {
            messageSystem.DisplayMessage("You cannot build here", Color.red);
            return;
        }
        if (wallet.CanAfford(structure.Cost) == false) {
            messageSystem.DisplayMessage("You cannot afford " + structure.Cost, Color.red);
            return;
        }

        //  Build structure
        BuildStructure(structure, position);
        wallet.SpendMoney(structure.Cost);
        messageSystem.DisplayMessage(string.Format("Spent {0}g", structure.Cost), Color.yellow);
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

        //  Refresh hover after building
        hoverManager.NewTileHovered(position, CurrentBuildMode, currentlySelectedStructure);

        if (OnStructureChanged != null) {
            OnStructureChanged.Invoke();
        }
    }

    private void AttemptDemolishStructure(Vector3Int position) {
        if (buildValidator.CanDemolishStructure(position) == false) {
            return;
        }
        DemolishStructure(position);

        //  Refresh hover after demolishing
        hoverManager.NewTileHovered(position, CurrentBuildMode, currentlySelectedStructure);
    }

    private void DemolishStructure(Vector3Int position) {
        StructureData structureAtPos = (StructureData)mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);
        float structureValue;

        if (structureAtPos.GetType() == typeof(TowerData)) {
            RaycastHit2D hit = Physics2D.Raycast((Vector3)position, -Vector2.up);
            objectPool.DestroyTower(hit.collider.GetComponent<Tower>());

            //  Mapmanager.GetTileData() will not return the currect tower type so we must get it from tower
            TowerData towerData = hit.collider.GetComponent<Tower>().TowerData;
            structureValue = Mathf.RoundToInt(towerData.Cost * wallet.GetResellPercentageInDecimal());
        }
        else {
            structureValue = Mathf.RoundToInt(structureAtPos.Cost * wallet.GetResellPercentageInDecimal());
        }

        mapManager.RemoveTile(IMapManager.Layer.StructureLayer, position);
        Debug.Log("sold structure for " + structureValue);
        wallet.GainMoney(structureValue);

        if (OnStructureChanged != null) {
            OnStructureChanged.Invoke();
        }
    }

    /// <summary>
    /// Handles logic when a new tile is hovered
    /// </summary>
    /// <param name="tileCoords"></param>
    private void HandleNewTileHovered(Vector3Int tileCoords) {
        lastPositionHovered = tileCoords;
        hoverManager.NewTileHovered(tileCoords, CurrentBuildMode, currentlySelectedStructure);

        if (ShouldShowRadius(tileCoords)) {
            radiusRenderer.RenderRadius(tileCoords + tilemapOffset, ((TowerData)(currentlySelectedStructure)).Range);
        }
        else {
            radiusRenderer.HideRadius();
        }
    }

    /// <summary>
    /// Instantiates and initializes a tower game object and adds it to list
    /// </summary>
    /// <param name="towerData"></param>
    /// <param name="position"></param>
    private void InstantiateTower(TowerData towerData, Vector3Int position) {
        Tower tower = objectPool.CreateTower(towerData.Type);
        tower.gameObject.transform.position = position + tilemapOffset;
    }

    /// <summary>
    /// Responds to a complete mouse click
    /// If in build mode, it will attempt to build a structure
    /// </summary>
    private void HandleMouseUp() {
        //  Prevent building/demolishing after game ended
        if (gameManager.GameEnded == true) {
            return;
        }

        //  If cursor isn't over a gui element
        if (EventSystem.current.IsPointerOverGameObject() == false) {
            if (CurrentBuildMode == BuildMode.Build) {
                AttemptBuildStructure(currentlySelectedStructure, lastPositionHovered);
            }
            else if (CurrentBuildMode == BuildMode.Demolish) {
                AttemptDemolishStructure(lastPositionHovered);
            }
        }

    }

    private bool ShouldShowRadius(Vector3Int tileCoords) {
        if (CurrentBuildMode == BuildMode.Build 
            && EventSystem.current.IsPointerOverGameObject() == false 
            && currentlySelectedStructure.GetType() == typeof(TowerData) 
            && mapManager.ContainsTileAt(IMapManager.Layer.GroundLayer, tileCoords)) {

            return true;
        }
        return false;
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
        currentlySelectedStructure = null;
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
}