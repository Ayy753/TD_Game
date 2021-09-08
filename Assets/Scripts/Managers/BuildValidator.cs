namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using UnityEngine;

    public class BuildValidator : IBuildValidator {
        readonly IMapManager mapManager;
        readonly IPathfinder pathfinder;

        public BuildValidator(IMapManager mapManager, IPathfinder pathfinder) {
            this.mapManager = mapManager;
            this.pathfinder = pathfinder;
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
            return mapManager.ContainsTileAt(IMapManager.Layer.GroundLayer, position);
        }

        private bool IsPositionEmpty(Vector3Int position) {
            return !mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position);
        }

        private bool IsGroundSolid(Vector3Int position) {
            return mapManager.IsGroundSolid(position);
        }

        private bool WouldBuildingBlockPath(Vector3Int position) {
            return pathfinder.WouldBlockPath(position);
        }

        public bool IsStructurePresentAndDemolishable(Vector3Int position) {
            StructureData structureData = (StructureData)mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);
            if (structureData != null && structureData.Demolishable) {
                return true;
            }
            return false;
        }

        private bool IsTowerAtOrAdjacent(Vector3Int position) {
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    Vector3Int neighbour = position + new Vector3Int(x, y, 0);
                    StructureData structureData = (StructureData)mapManager.GetTileData(IMapManager.Layer.StructureLayer, neighbour);
                    if (structureData != null && structureData.GetType() == typeof(TowerData)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
