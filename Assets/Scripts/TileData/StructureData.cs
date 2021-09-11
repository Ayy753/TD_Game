namespace DefaultNamespace.TilemapSystem {

    public abstract class StructureData : TileData {
        public override MapLayer Layer { get; protected set; } = MapLayer.StructureLayer;
        public abstract int Cost { get; protected set; }
        public virtual bool Demolishable { get; protected set; } = true;
        public virtual bool Buildable { get; protected set; } = true;
    }
}
