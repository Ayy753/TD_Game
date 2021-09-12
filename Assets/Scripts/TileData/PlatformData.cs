namespace DefaultNamespace.TilemapSystem {

    using UnityEngine;
    using UnityEngine.Tilemaps;

    [CreateAssetMenu(fileName = "New Platform Tile", menuName = "Platform Tile")]
    public class PlatformData : StructureData {
        [field: SerializeField]
        public override string Name { get; protected set; }
        [field: SerializeField]
        public override string Description { get; protected set; }
        [field: SerializeField]
        public override Sprite Icon { get; protected set; }
        [field: SerializeField]
        public override TileBase TileBase { get; protected set; }
        [field: SerializeField]
        public float WalkCost { get; protected set; }
        public override MapLayer Layer { get; protected set; } = MapLayer.PlatformLayer;
        [field: SerializeField]
        public override int Cost { get; protected set; }
        [field: SerializeField]
        public override bool Buildable { get; protected set; }
        [field: SerializeField]
        public bool CanBeBuiltOn { get; protected set; }
        public override string ToString() {
            return base.ToString() +
                $"\n<b>Walk cost</b>: {WalkCost}" +
                $"\n<b>Can be built on</b>: {CanBeBuiltOn}" +
                $"\n<b>Cost</b>: {Cost}";
        }
    }
}
