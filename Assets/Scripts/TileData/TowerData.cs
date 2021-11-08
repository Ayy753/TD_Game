namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using DefaultNamespace.TilemapSystem;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [CreateAssetMenu(fileName = "New Tower", menuName = "Tower")]
    public class TowerData : StructureData {
        [field: SerializeField]
        public override string Name { get; protected set; }
        [field: SerializeField]
        public override string Description { get; protected set; }
        [field: SerializeField]
        public override Sprite Icon { get; protected set; }
        [field: SerializeField]
        public float Range { get; private set; }
        [field: SerializeField]
        public override TileBase TileBase { get; protected set; }
        [field: SerializeField]
        public override int Cost { get; protected set; }
        //[field: SerializeField]
        public EffectGroup EffectGroup { get; protected set; }
        [field: SerializeField]
        public string ProjectileName { get; protected set; }
        [field: SerializeField]
        public LayerMask TargetMask { get; protected set; }

        public override string ToString() {
            return base.ToString()
                + string.Format("\n<b>Range</b>: {0}m" +
                "\n<b>Cost</b>: {1}g" +
                "\n<b>Reload time</b>: {2}s" +
                "\n<b>Total Damage</b>: {3}" +
                "\n\n<b>Projectile Effects</b>:\n{4}", Range, Cost, EffectGroup.Cooldown, EffectGroup.GetTotalDamage(), EffectGroup.GetEffectInfo());
        }

        public void SetEffectGroup(EffectGroup effectGroup) {
            EffectGroup = effectGroup;
        }
    }
}
