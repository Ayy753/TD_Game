public abstract class StructureData : TileData
{
    public override IMapManager.Layer Layer { get; protected set; } = IMapManager.Layer.StructureLayer;
    public abstract int Cost { get; protected set; }  
}