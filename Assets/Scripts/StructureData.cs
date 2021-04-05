public abstract class StructureData : TileData
{
    public override MapManager.Layer Layer { get; protected set; } = MapManager.Layer.StructureLayer;
    public abstract int Cost { get; protected set; }  
}