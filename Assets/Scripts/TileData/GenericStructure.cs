using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Generic Structure", menuName = "GenericStructure")]
public class GenericStructure : StructureData {
    [field: SerializeField]
    public override int Cost { get; protected set; }
    [field: SerializeField]
    public override string Name { get; protected set; }
    [field: SerializeField]
    public override string Description { get; protected set; }
    [field: SerializeField]
    public override Sprite Icon { get; protected set; }
    [field: SerializeField]
    public override TileBase TileBase { get; protected set; }
    [field: SerializeField]
    public override bool Demolishable { get; protected set; }
    [field: SerializeField]
    public override bool Buildable { get; protected set; }
}
