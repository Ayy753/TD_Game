using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="New Tower", menuName ="Tower")]
public class TowerData : StructureData
{
    [field: SerializeField]
    public override string Name { get; protected set; }
    [field: SerializeField]
    public override string Description { get; protected set; }
    [field: SerializeField]
    public override Sprite Icon { get; protected set; }
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
    [field: SerializeField]
    public float Damage { get; private set; }
    [field: SerializeField]
    public float Range { get; private set; }
    [field: SerializeField]
    public float ReloadTime { get; set; }
    [field: SerializeField]
    public override TileBase TileBase { get; protected set; }

    public override string ToString()
    {
        return base.ToString() + string.Format("\nDamage: {0}\nRange: {1}", Damage, Range);
    }
}
