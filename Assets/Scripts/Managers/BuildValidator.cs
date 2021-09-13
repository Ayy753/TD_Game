namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using UnityEngine;

    public enum StructureBuildError {
        None,
        NoGround,
        GroundUnstable,
        StructurePresent,
        BlockingPath,
        TowerAdjacent
    }

    public enum PlatformBuildError {
        None,
        NoGround,
        AlreadyContainsPlatform,
        GroundStable
    }

    public class BuildValidator : IBuildValidator {
        readonly IMapManager mapManager;
        readonly IPathfinder pathfinder;

        public BuildValidator(IMapManager mapManager, IPathfinder pathfinder) {
            this.mapManager = mapManager;
            this.pathfinder = pathfinder;
        }

        public StructureBuildError ValidateStructureBuildabilityOverPosition(Vector3Int position, StructureData structureData) {
            StructureBuildError buildError = ValidatePositionBuildability(position);
            if (buildError != StructureBuildError.None){
                return buildError;
            }
            else if(structureData is TowerData && IsTowerAtOrAdjacent(position)) {
                return StructureBuildError.TowerAdjacent;
            }
            else {
                return StructureBuildError.None;
            }
        }

        public StructureBuildError ValidatePositionBuildability(Vector3Int position) {
            if (!DoesTileContainGround(position)) {
                return StructureBuildError.NoGround;
            }
            else if (!IsGroundSolid(position)) {
                return StructureBuildError.GroundUnstable;
            }
            else if (!IsPositionEmpty(position)) {
                return StructureBuildError.StructurePresent;
            }
            else if (WouldBuildingBlockPath(position)) {
                return StructureBuildError.BlockingPath;
            }
            else return StructureBuildError.None;
        }

        public bool CanBuildStructureOverPosition(Vector3Int position, StructureData structureData) {
            if (!IsPositionBuildable(position)) {
                return false;
            }

            if (structureData is TowerData
                && IsTowerAtOrAdjacent(position)) {
                return false;
            }
            return true;
        }

        public bool IsPositionBuildable(Vector3Int position) {
            return DoesTileContainGround(position)
                && IsGroundSolid(position)
                && IsPositionEmpty(position)
                && !WouldBuildingBlockPath(position);
        }

        private bool DoesTileContainGround(Vector3Int position) {
            return mapManager.ContainsTileAt(MapLayer.GroundLayer, position);
        }

        private bool IsPositionEmpty(Vector3Int position) {
            return !mapManager.ContainsTileAt(MapLayer.StructureLayer, position);
        }

        private bool IsGroundSolid(Vector3Int position) {
            return mapManager.CanGroundOrPlatformBeBuiltOn(position);
        }

        private bool WouldBuildingBlockPath(Vector3Int position) {
            return pathfinder.WouldBlockPath(position);
        }

        public bool IsStructurePresentAndDemolishable(Vector3Int position) {
            StructureData structureData = (StructureData)mapManager.GetTileData(MapLayer.StructureLayer, position);
            if (structureData != null && structureData.Demolishable) {
                return true;
            }
            return false;
        }

        private bool IsTowerAtOrAdjacent(Vector3Int position) {
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    Vector3Int neighbour = position + new Vector3Int(x, y, 0);
                    StructureData structureData = (StructureData)mapManager.GetTileData(MapLayer.StructureLayer, neighbour);
                    if (structureData != null && structureData.GetType() == typeof(TowerData)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanBuildPlatformOverPosition(Vector3Int position) {
            return ValidatePlatformBuildabilityOverPosition(position) == PlatformBuildError.None;
        }

        public PlatformBuildError ValidatePlatformBuildabilityOverPosition(Vector3Int position) {
            if (!DoesTileContainGround(position)) {
                return PlatformBuildError.NoGround;
            }
            else if (DoesTileContainPlatform(position)) {
                return PlatformBuildError.AlreadyContainsPlatform;
            }
            else if (IsGroundSolid(position)) {
                return PlatformBuildError.GroundStable;
            }
            else {
                return PlatformBuildError.None;
            }
        }

        private bool DoesTileContainPlatform(Vector3Int position) {
            return mapManager.ContainsTileAt(MapLayer.PlatformLayer, position);
        }

        public bool IsPlatformPresentAndDemolishable(Vector3Int position) {
            if (DoesTileContainPlatform(position)) {
                PlatformData platform = (PlatformData)mapManager.GetTileData(MapLayer.PlatformLayer, position);
                if (platform.Demolishable) {
                    return true;
                }
            }
            return false;
        }
    }
}
