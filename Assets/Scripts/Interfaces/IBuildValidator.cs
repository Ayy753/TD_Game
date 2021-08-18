namespace DefaultNamespace {

    using DefaultNamespace.TilemapSystem;
    using UnityEngine;

    public interface IBuildValidator {
        /// <summary>
        /// Checks if user can build over a postion, and validates structure-specific conditions
        /// </summary>
        /// <param name="position"></param>
        /// <param name="structureData"></param>
        /// <returns></returns>
        public bool CanBuildStructureOverPosition(Vector3Int position, StructureData structureData);

        /// <summary>
        /// Checks if user can demolish a structure over a position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsStructurePresentAndDemolishable(Vector3Int position);

        /// <summary>
        /// Checks if the user can build a generic structure over a position. Does not validate special structure-specific conditions
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsPositionBuildable(Vector3Int position);
    }
}
