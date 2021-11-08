namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using DefaultNamespace.TilemapSystem;
    using System.Collections.Generic;
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
            return base.ToString() +
                $"\n<b>Target Types</b>: {TargetTypes()}" +
                $"\n<b>Range</b>: {Range}m" +
                $"\n<b>Cost</b>: {Cost}g" +
                $"\n<b>Reload time</b>: {EffectGroup.Cooldown}s" +
                $"\n<b>Total Damage</b>: {EffectGroup.GetTotalDamage()}" +
                $"\n\n<b>Projectile Effects</b>:\n{EffectGroup.GetEffectInfo()}";
        }

        private string TargetTypes() {
            List<string> targetTypes = new List<string>();

            if ((TargetMask & 1 << LayerMask.NameToLayer("Invisible")) != 0) {
                targetTypes.Add("Invisible");
            }
            if ((TargetMask & 1 << LayerMask.NameToLayer("Ground")) != 0) {
                targetTypes.Add("Ground");
            }
            if ((TargetMask & 1 << LayerMask.NameToLayer("Flying")) != 0) {
                targetTypes.Add("Flying");
            }

            return string.Join(", ", targetTypes.ToArray());
        }

        public void SetEffectGroup(EffectGroup effectGroup) {
            EffectGroup = effectGroup;
        }
    }
}
