using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Ground Tile", menuName = "Ground Tile")]
public class GroundData : TileData
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
    public override MapManager.Layer Layer { get; protected set; } = MapManager.Layer.GroundLayer;
}
