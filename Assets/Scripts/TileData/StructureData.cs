public abstract class StructureData : TileData
{
    public override IMapManager.Layer Layer { get; protected set; } = IMapManager.Layer.StructureLayer;
    public abstract int Cost { get; protected set; }
    public bool Demolishable { get; protected set; } = true;
}