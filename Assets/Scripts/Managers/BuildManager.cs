using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BuildManager : IInitializable, IDisposable {

    #region properties
    public BuildMode CurrentBuildMode { get; private set; } = BuildMode.None;
    public StructureData CurrentlySelectedStructure { get; private set; }
    #endregion

    #region fields
    IMapManager mapManager;
    IBuildValidator buildValidator;
    IMessageSystem messageSystem;
    IWallet wallet;
    IWaveManager waveManager;
    GameManager gameManager;
    ObjectPool objectPool;
    RadiusRenderer radiusRenderer;

    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);
    private Vector3Int lastPositionHovered;

    public static EventHandler<StructureChangedEventArgs> StructureChanged;

    #endregion

    public enum BuildMode {
        Build,
        Demolish,
        None
    }

    public BuildManager(IMapManager mapManager, GameManager gameManager, IBuildValidator buildValidator, IMessageSystem messageSystem, IWallet wallet, ObjectPool objectPool, RadiusRenderer radiusRenderer, IWaveManager waveManager) {
        Debug.Log("build manager constructor");
        this.mapManager = mapManager;
        this.gameManager = gameManager;
        this.buildValidator = buildValidator;
        this.messageSystem = messageSystem;
        this.wallet = wallet;
        this.objectPool = objectPool;
        this.radiusRenderer = radiusRenderer;
        this.waveManager = waveManager;
    }

    public void Initialize() {
        MouseManager.OnHoveredNewTile += HandleNewTileHovered;
        MouseManager.OnLeftMouseUp += HandleLeftMouseUp;
        MouseManager.OnRightMouseUp += HandleRightMouseUp;
        waveManager.OnWaveStateChanged += HandleWaveStateChanged;
    }

    public void Dispose() {
        MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
        MouseManager.OnLeftMouseUp -= HandleLeftMouseUp;
        MouseManager.OnRightMouseUp -= HandleRightMouseUp;
        waveManager.OnWaveStateChanged -= HandleWaveStateChanged;
    }

    private void TryToBuyAndBuildStructureAndDisplayMessages(StructureData structureData, Vector3Int position) {
        if (CanAffordStructure(structureData) == false) {
            messageSystem.DisplayMessage("You cannot afford " + structureData.Cost, Color.red);
            return;
        }
        else if (CanBuildStructureAtPosition(structureData, position) == false) {
            messageSystem.DisplayMessage("You cannot build here", Color.red);
            return;
        }
        else {
            BuyStructure(structureData);
            BuildStructure(structureData, position);
            messageSystem.DisplayMessageAtCursor(string.Format("Spent {0}g", structureData.Cost), Color.yellow);
        }
    }

    private bool CanAffordStructure(StructureData structureData) {
        return wallet.CanAfford(structureData.Cost);
    }

    private bool CanBuildStructureAtPosition(StructureData structureData, Vector3Int position) {
        return buildValidator.CanBuildStructureOverPosition(position, structureData);
    }

    private void BuyStructure(StructureData structureData) {
        wallet.SpendMoney(structureData.Cost);
    }

    private void BuildStructure(StructureData structureData, Vector3Int position) {
        if (structureData is TowerData) {
            InstantiateTower((TowerData)structureData, position);
        }

        mapManager.SetTile(position, structureData);
        StructureChanged.Invoke(this, new StructureChangedEventArgs(StructureChangedEventArgs.Type.build, position));
    }

    private void AttemptDemolishStructure(Vector3Int position) {
        if (buildValidator.CanDemolishStructure(position) == false) {
            return;
        }
        DemolishStructure(position);
    }

    private void DemolishStructure(Vector3Int position) {
        StructureData structureAtPos = (StructureData)mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);
        float structureValue;

        if (structureAtPos.GetType() == typeof(TowerData)) {
            Tower tower = GetTower(position);
            DestroyTower(tower);

            structureValue = Mathf.RoundToInt(tower.TowerData.Cost * wallet.GetResellPercentageInDecimal());
        }
        else {
            structureValue = Mathf.RoundToInt(structureAtPos.Cost * wallet.GetResellPercentageInDecimal());
        }

        mapManager.RemoveTile(IMapManager.Layer.StructureLayer, position);
        Debug.Log("sold structure for " + structureValue);
        wallet.GainMoney(structureValue);
        messageSystem.DisplayMessageAtCursor(string.Format("+{0}g", structureValue), Color.yellow);

        StructureChanged.Invoke(this, new StructureChangedEventArgs(StructureChangedEventArgs.Type.demolish, position));
    }

    /// <summary>
    /// Handles logic when a new tile is hovered
    /// </summary>
    /// <param name="tileCoords"></param>
    private void HandleNewTileHovered(Vector3Int tileCoords) {
        lastPositionHovered = tileCoords;

        Vector3 coordsWithOffset = tileCoords + tilemapOffset;

        if (ShouldShowRadius(tileCoords)) {
            if (CurrentBuildMode == BuildMode.Build) {
                radiusRenderer.RenderRadius(coordsWithOffset, ((TowerData)(CurrentlySelectedStructure)).Range);
            }
            else {
                radiusRenderer.RenderRadius(coordsWithOffset, GetTower(tileCoords).TowerData.Range);
            }
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
    /// Removes tower from the scene
    /// </summary>
    /// <param name="tower"></param>
    private void DestroyTower(Tower tower) {
        tower.IsBeingDestroyed();
        objectPool.DestroyTower(tower);
    }

    /// <summary>
    /// Responds to a complete mouse click
    /// If in build mode, it will attempt to build a structure
    /// </summary>
    private void HandleLeftMouseUp() {
        if (HasGameEnded()) {
            return;
        }

        if (IsMouseOverGUI() == false) {
            if (CurrentBuildMode == BuildMode.Build) {
                TryToBuyAndBuildStructureAndDisplayMessages(CurrentlySelectedStructure, lastPositionHovered);
            }
            else if (CurrentBuildMode == BuildMode.Demolish) {
                AttemptDemolishStructure(lastPositionHovered);
            }
        }
    }

    private bool IsMouseOverGUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool HasGameEnded() {
        if (gameManager.CurrentState == GameManager.State.Ended) {
            return true;
        }
        return false;
    }
    
    private void HandleRightMouseUp() {
        //  Prevent building/demolishing after game ended
        if (gameManager.CurrentState == GameManager.State.Ended) {
            return;
        }

        //  If cursor isn't over a gui element
        if (EventSystem.current.IsPointerOverGameObject() == false) {
            ExitBuildMode();
        }

    }

    private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
        if (arg.newState == IWaveManager.State.WaveInProgress && CurrentBuildMode != BuildMode.None) {
            ExitBuildMode();
        }
    }

    /// <summary>
    /// Returns true if user is in build mode and selected a tower and cursor is over a ground tile
    /// Or if user is not in build mode and cursor is over a tower 
    /// </summary>
    /// <param name="tileCoords"></param>
    /// <returns></returns>
    private bool ShouldShowRadius(Vector3Int tileCoords) {
        if (CurrentBuildMode == BuildMode.Build 
            && EventSystem.current.IsPointerOverGameObject() == false 
            && CurrentlySelectedStructure.GetType() == typeof(TowerData) 
            && mapManager.ContainsTileAt(IMapManager.Layer.GroundLayer, tileCoords)) {

            return true;
        }
        else if (CurrentBuildMode == BuildMode.None){
            TileData tileData = mapManager.GetTileData(IMapManager.Layer.StructureLayer, tileCoords);
            if (tileData != null && tileData.GetType() == typeof(TowerData)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Finds and returns a tower at a postiion if one exists 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Tower GetTower(Vector3 position) {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if (hit.collider != null) {
            return hit.collider.GetComponent<Tower>();
        }
        return null;
    }

    /// <summary>
    /// Enters build mode
    /// </summary>
    /// <param name="selectedStructure"></param>
    public void EnterBuildMode(StructureData selectedStructure) {
        CurrentlySelectedStructure = selectedStructure;

        CurrentBuildMode = BuildMode.Build;
        Debug.Log("Entered buildmode for structure: " + selectedStructure.name);
    }

    /// <summary>
    /// Enters demolish mode
    /// </summary>
    public void EnterDemolishMode() {

        CurrentBuildMode = BuildMode.Demolish;
        CurrentlySelectedStructure = null;
        Debug.Log("Entered demolish mode");
    }

    /// <summary>
    /// Exits build/demolish mode
    /// </summary>
    public void ExitBuildMode() {
        CurrentlySelectedStructure = null;
        CurrentBuildMode = BuildMode.None;
        radiusRenderer.HideRadius();
        Debug.Log("Exited build mode");
    }

    public void SellTower(Tower tower) {
        Vector3Int pos = Vector3Int.FloorToInt(tower.gameObject.transform.position);
        DemolishStructure(pos);
    }
}

public class StructureChangedEventArgs : EventArgs {
    public Type changeType;
    public Vector3Int position;

    public StructureChangedEventArgs(Type changeType, Vector3Int position) {
        this.changeType = changeType;
        this.position = position;
    }
    
    public enum Type {
        build,
        demolish
    }
}