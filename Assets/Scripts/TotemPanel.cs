namespace DefaultNamespace.GUI {

    using DefaultNamespace;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class TotemPanel : IInitializable {
        GameObject pnlTotem;
        private TMP_Text txtTotemName, txtTotemDescription;
        private Totem targetTotem;

        public void Initialize() {
            pnlTotem = GameObject.Find("pnlTotem");
            txtTotemName = GameObject.Find("txtTotemName").GetComponent<TMP_Text>();
            txtTotemDescription = GameObject.Find("txtTotemDescription").GetComponent<TMP_Text>();

            pnlTotem.SetActive(false);
        }

        public void UpdateTotemPanel() {
            TotemData totemData = targetTotem.totemData;

            txtTotemName.text = targetTotem.GetName();
            string descriptionText = string.Format(
                "<b>Radius</b>: {0}m\n" +
                "<b>Recharge time</b>: {1}s" + "\n" +
                "<b>Area Effects</b>: {2}", totemData.EffectGroup.Radius, totemData.EffectGroup.Cooldown, totemData.EffectGroup.GetEffectInfo());

            txtTotemDescription.text = descriptionText;
        }

        public void TargetTotem(Totem totem) {
            pnlTotem.SetActive(true);
            targetTotem = totem;
            UpdateTotemPanel();
        }

        internal void ClearTarget() {
            pnlTotem.SetActive(false);
            targetTotem = null;
        }
    }
}
