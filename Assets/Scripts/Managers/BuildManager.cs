namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Zenject;

    public enum BuildMode {
        Build,
        Demolish,
        None
    }

    public class StructureChangedEventArgs : EventArgs {
        public BuildMode ChangeType { get; private set; }
        public Vector3Int Position { get; private set; }

        public StructureChangedEventArgs(BuildMode changeType, Vector3Int position) {
            ChangeType = changeType;
            Position = position;
        }
    }

    public class BuildManager : IInitializable, IDisposable {
        private readonly IMapManager mapManager;
        private readonly IBuildValidator buildValidator;
        private readonly IMessageSystem messageSystem;
        private readonly IWallet wallet;
        private readonly IWaveManager waveManager;
        private readonly GameManager gameManager;
        private readonly ObjectPool objectPool;
        private readonly RadiusRenderer radiusRenderer;

        public BuildMode CurrentBuildMode { get; private set; } = BuildMode.None;
        public StructureData CurrentlySelectedStructure { get; private set; }

        private readonly Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);
        private Vector3Int lastPositionHovered;

        public static event EventHandler<StructureChangedEventArgs> OnStructureChanged;

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

        private void HandleLeftMouseUp() {
            if (!HasGameEnded() && !IsMouseOverGUI()) {
                if (CurrentBuildMode == BuildMode.Build) {
                    TryToBuyAndBuildStructureAndDisplayMessages(CurrentlySelectedStructure, lastPositionHovered);
                }
                else if (CurrentBuildMode == BuildMode.Demolish) {
                    TryToDemolishAndSellStructureAndDisplayMessages(lastPositionHovered);
                }
            }
        }

        private bool HasGameEnded() {
            if (gameManager.CurrentState == GameState.GameWon || gameManager.CurrentState == GameState.GameLost) {
                return true;
            }
            return false;
        }

        private bool IsMouseOverGUI() {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private void HandleRightMouseUp() {
            if (!IsMouseOverGUI()) {
                ExitBuildOrDemolishMode();
            }
        }

        private void TryToBuyAndBuildStructureAndDisplayMessages(StructureData structureData, Vector3Int position) {
            if (!CanAffordStructure(structureData)) {
                messageSystem.DisplayMessage("You cannot afford " + structureData.Cost, Color.red);
            }
            else{
                StructureBuildError buildError = ValidateStructureBuildAtPosition(structureData, position);
                if (buildError != StructureBuildError.None) {
                    messageSystem.DisplayMessage(GetBuildErrorMessage(buildError), Color.red);
                }
                else {
                    BuyStructure(structureData);
                    BuildStructure(structureData, position);
                    messageSystem.DisplayMessageAtCursor(string.Format("Spent {0}g", structureData.Cost), Color.yellow);
                }
            }
        }

        private bool CanAffordStructure(StructureData structureData) {
            return wallet.CanAfford(structureData.Cost);
        }

        private StructureBuildError ValidateStructureBuildAtPosition(StructureData structureData, Vector3Int position) {
            return buildValidator.ValidateStructureBuildabilityOverPosition(position, structureData);
        }

        private string GetBuildErrorMessage(StructureBuildError buildError) {
            switch (buildError) {
                case StructureBuildError.NoGround:
                    return "There is no ground to build on";
                case StructureBuildError.GroundUnstable:
                    return "The ground is too unstable to build on";
                case StructureBuildError.StructurePresent:
                    return "There is a structure present here";
                case StructureBuildError.BlockingPath:
                    return "You cannot block the path";
                case StructureBuildError.TowerAdjacent:
                    return "You cannot build towers next to each other";
                default:
                    Debug.LogError("Build error is invalid");
                    return string.Empty;
            }
        }

        private void BuyStructure(StructureData structureData) {
            wallet.SpendMoney(structureData.Cost);
        }

        private void BuildStructure(StructureData structureData, Vector3Int position) {
            if (structureData is TowerData towerData) {
                InstantiateTowerGameObjectAtPosition(towerData, position);
            }

            mapManager.SetTile(position, structureData);
            OnStructureChanged.Invoke(null, new StructureChangedEventArgs(BuildMode.Build, position));
        }

        private void TryToDemolishAndSellStructureAndDisplayMessages(Vector3Int position) {
            if (!IsDemolishableStructureAtPosition(position)) {
                messageSystem.DisplayMessage("There is no demolishable structure here", Color.red);
            }
            else {
                StructureData structureAtPosition = GetStructureDataAtPosition(position);
                float sellValue = GetSellValue(structureAtPosition);

                SellStructure(structureAtPosition);
                DemolishStructureAtPosition(position);
                messageSystem.DisplayMessageAtCursor(string.Format("+{0}g", sellValue), Color.yellow);
            }
        }

        private bool IsDemolishableStructureAtPosition(Vector3Int position) {
            return buildValidator.IsStructurePresentAndDemolishable(position);
        }

        private StructureData GetStructureDataAtPosition(Vector3Int position) {
            return (StructureData)mapManager.GetTileData(MapLayer.StructureLayer, position);
        }

        private float GetSellValue(StructureData structureData) {
            float sellValue = structureData.Cost * wallet.GetResellPercentageInDecimal();
            float sellValueRounded = (float)Math.Round(sellValue, 0);
            return sellValueRounded;
        }

        private void SellStructure(StructureData structureData) {
            float sellValue = GetSellValue(structureData);
            wallet.GainMoney(sellValue);
        }

        private void DemolishStructureAtPosition(Vector3Int position) {
            StructureData structureData = GetStructureDataAtPosition(position);

            if (structureData is TowerData) {
                Tower tower = GetTowerAtPosition(position);
                DestroyTower(tower);
            }

            mapManager.RemoveTile(MapLayer.StructureLayer, position);
            OnStructureChanged.Invoke(null, new StructureChangedEventArgs(BuildMode.Build, position));
        }

        private void HandleNewTileHovered(Vector3Int position) {
            lastPositionHovered = position;

            if (ShouldShowTowerRadiusAtPosition(position)) {
                try {
                    ShowTowerRadiusAtPosition(position);
                }
                catch (Exception e) {
                    Debug.LogError($"{e}, a raycast target object must be overlapping the tower's center");
                }
            }
            else {
                HideTowerRadius();
            }
        }

        private bool ShouldShowTowerRadiusAtPosition(Vector3Int position) {
            if ((CurrentBuildMode == BuildMode.Build && CurrentlySelectedStructure is TowerData) || IsTowerPresentAtPosition(position)) {
                return true;
            }
            return false;
        }

        private bool IsTowerPresentAtPosition(Vector3Int position) {
            Vector3 positionWithOffset = position + tilemapOffset;

            if (GetTowerAtPosition(positionWithOffset) != null) {
                return true;
            }
            return false;
        }

        private Tower GetTowerAtPosition(Vector3 position) {
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
            if (hit.collider != null) {
                return hit.collider.GetComponent<Tower>();
            }
            return null;
        }

        private void ShowTowerRadiusAtPosition(Vector3Int position) {
            Vector3 positionWithOffset = position + tilemapOffset;

            if (CurrentBuildMode == BuildMode.Build && CurrentlySelectedStructure is TowerData towerData) {
                radiusRenderer.RenderRadius(positionWithOffset, towerData.Range);
            }
            else {
                radiusRenderer.RenderRadius(positionWithOffset, GetTowerAtPosition(positionWithOffset).TowerData.Range);
            }
        }

        private void HideTowerRadius() {
            radiusRenderer.HideRadius();
        }

        private void InstantiateTowerGameObjectAtPosition(TowerData towerData, Vector3Int position) {
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

        private void HandleWaveStateChanged(object sender, WaveStateChangedEventArgs arg) {
            if (arg.NewState == WaveState.WaveInProgress && CurrentBuildMode != BuildMode.None) {
                ExitBuildOrDemolishMode();
            }
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
        public void ExitBuildOrDemolishMode() {
            CurrentlySelectedStructure = null;
            CurrentBuildMode = BuildMode.None;
            HideTowerRadius();
            Debug.Log("Exited build mode");
        }

        public void SellTower(Tower tower) {
            Vector3Int positiion = Vector3Int.FloorToInt(tower.gameObject.transform.position);
            TryToDemolishAndSellStructureAndDisplayMessages(positiion);
        }
    }
}
