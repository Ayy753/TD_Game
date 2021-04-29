using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Wall Tile", menuName = "Wall Tile")]
public class WallData : StructureData
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
    public override int Cost { get; protected set; }

    public override string ToString()
    {
        return base.ToString() + string.Format("\n<b>Cost</b>: {0}", Cost);
    }
}
