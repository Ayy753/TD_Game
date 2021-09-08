namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using UnityEngine;

    public interface IBuildValidator {
        public bool IsStructurePresentAndDemolishable(Vector3Int position);
        public bool IsPositionBuildable(Vector3Int position);
        public bool CanBuildStructureOverPosition(Vector3Int position, StructureData structureData);
        public StructureBuildError ValidateStructureBuildabilityOverPosition(Vector3Int position, StructureData structureData);
        public StructureBuildError ValidatePositionBuildability(Vector3Int position);
    }
}
