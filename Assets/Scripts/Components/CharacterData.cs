namespace DefaultNamespace {

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

        public override string ToString() {
            return string.Format("" +
                "<b>Health</b>: {0}\n" +
                "<b>FireResist</b>: {1}\n" +
                "<b>ColdResist</b>: {2}\n" +
                "<b>PoisonResist</b>: {3}\n" +
                "<b>LightningResist</b>: {4}\n" +
                "<b>Armor</b>: {5}\n" +
                "<b>Speed</b>: {6}\n",
                BaseHealth, BaseFireResist, BaseColdResist, BasePoisonResist, BaseLightningResist, BaseArmor, BaseSpeed);
        }
    }
}
