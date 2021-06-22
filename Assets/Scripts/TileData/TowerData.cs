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
    public TowerType Type { get; protected set; }
    [field: SerializeField]
    public float Range { get; private set; }
    [field: SerializeField]
    public float ReloadTime { get; set; }
    [field: SerializeField]
    public override TileBase TileBase { get; protected set; }
    [field: SerializeField]
    public override int Cost { get; protected set; }
    [field: SerializeField]
    public ProjectileData ProjectileData { get; protected set; }


    [SerializeReference]
    public IEffect[] effects;

    public enum TowerType {
        Bullet,
        Splash,
        Sniper
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("\n<b>Raw Damage</b>: {0}\n<b>Range</b>: {1}\n<b>Cost</b>: {2}\n<b>Shots per second</b>: {3}", ProjectileData.RawTotalDamage(), Range, Cost, 1/ReloadTime);
    }
}
