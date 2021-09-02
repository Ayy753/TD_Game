namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Character", menuName = "Character Data")]
    public class CharacterData : ScriptableObject {
        [field: SerializeField] public float BaseHealth { get; private set; }
        [field: SerializeField] public float BaseFireResist { get; private set; }
        [field: SerializeField] public float BaseColdResist { get; private set; }
        [field: SerializeField] public float BasePoisonResist { get; private set; }
        [field: SerializeField] public float BaseLightningResist { get; private set; }
        [field: SerializeField] public float BaseArmor { get; private set; }
        [field: SerializeField] public float BaseSpeed { get; private set; }
        [field: SerializeField] public string AbilityName { get; private set; }

        public EffectGroup EffectGroup { get; private set; }

        public override string ToString() {
            string effectGroupInfo = EffectGroup == null ? "None" : EffectGroup.ToString();
            string result = 
                $"<b>Health</b>: {BaseHealth}\n" +
                $"<b>FireResist</b>: {BaseFireResist}\n" +
                $"<b>ColdResist</b>: {BaseColdResist}\n" +
                $"<b>PoisonResist</b>: {BasePoisonResist}\n" +
                $"<b>LightningResist</b>: {BaseLightningResist}\n" +
                $"<b>Armor</b>: {BaseArmor}\n" +
                $"<b>Speed</b>: {BaseSpeed}\n" +
                $"<b>Ability</b>: {effectGroupInfo}";
            return result;
        }

        public void SetEffectGroup(EffectGroup effectGroup) {
            if (effectGroup.Type == TargetType.Individual) {
                Debug.LogError("Individual targetting effects not supported");
            }
            else {
                EffectGroup = effectGroup;
            }
        }
    }
}
