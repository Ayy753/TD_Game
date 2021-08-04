using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Totem", menuName = "Totem")]
public class TotemData : StructureData{
    [field: SerializeField] public override string Name { get; protected set; }
    [field: SerializeField] public float EffectDelay { get; protected set; }
    [field: SerializeField] public string EffectName { get; protected set; }
    public EffectGroup EffectGroup { get; private protected set; }
    public override int Cost { get; protected set; } = 0;
    [field: SerializeField] public override string Description { get; protected set; }
    [field: SerializeField] public override Sprite Icon { get; protected set; }
    [field: SerializeField] public override TileBase TileBase { get; protected set; }
    public override bool Demolishable { get; protected set; } = false;
    public override bool Buildable { get; protected set; } = false;
    public void SetEffectGroup(EffectGroup effectGroup) {
        EffectGroup = effectGroup;
    }
}
