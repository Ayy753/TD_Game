namespace DefaultNamespace {
    using System.Text;
    using UnityEngine;

    public class EnemyIcon : MonoBehaviour, IDisplayable {
        public EnemyData EnemyData { get; set; }
        private float healthModifier;
        private float valueModifier;

        public string GetDisplayText() {

            float modifiedHealth = Mathf.Round(EnemyData.BaseHealth + EnemyData.BaseHealth * healthModifier);
            float modifiedValue = Mathf.Round(EnemyData.BaseValue + EnemyData.BaseValue * valueModifier);
            string ability = EnemyData.EffectGroup != null ? EnemyData.EffectGroup.GetEffectInfo() : "None";

            StringBuilder sb = new StringBuilder();

            sb.Append($"<b>Health</b>: { modifiedHealth }\n");
            sb.Append($"<b>FireResist</b>: {EnemyData.BaseFireResist}\n");
            sb.Append($"<b>ColdResist</b>: {EnemyData.BaseColdResist}\n");
            sb.Append($"<b>PoisonResist</b>: {EnemyData.BasePoisonResist}\n");
            sb.Append($"<b>LightningResist</b>: {EnemyData.BaseLightningResist}\n");
            sb.Append($"<b>Armor</b>: {EnemyData.BaseArmor}\n");
            sb.Append($"<b>Speed</b>: {EnemyData.BaseSpeed}\n");
            sb.Append($"<b>Value</b>: { modifiedValue }\n");
            sb.Append($"<b>Ability</b>: { ability }\n");

            return sb.ToString();
        }

        public void SetHealthModifier(float healthModifier) {
            this.healthModifier = healthModifier;
        }

        public void SetValueModifier(float valueModifier) {
            this.valueModifier = valueModifier;
        }
    }
}
