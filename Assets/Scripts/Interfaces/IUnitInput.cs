namespace DefaultNamespace {

    using UnityEngine;

    public interface IUnitInput {
        public void ReachedNextTile();
        public Vector3Int GetNextTile();
    }
}
