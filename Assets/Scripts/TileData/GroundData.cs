using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Ground Tile", menuName = "Ground Tile")]
public class GroundData : TileData, IDisplayable
{
    [field: SerializeField]
    public override string Name { get; protected set; }
    [field: SerializeField]
    public override string Description { get; protected set; }
    [field: SerializeField]
    public override Sprite Icon { get; protected set; }
    [field: SerializeField]
    public override TileBase TileBase { get; protected set; }
    [field: SerializeField]
    public float WalkCost { get; private set; }
    [field: SerializeField]
    public bool IsSolid { get; private set; }
    public override IMapManager.Layer Layer { get; protected set; } = IMapManager.Layer.GroundLayer;
    public string GetDisplayText()
    {
        return ToString();
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("\n<b>Walk cost</b>: {0}\n<b>Solid?</b>: {1}", WalkCost, IsSolid);
    }
}