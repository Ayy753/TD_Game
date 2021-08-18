namespace DefaultNamespace.EffectSystem {
    using DefaultNamespace.StatusSystem;

    public class StatMod : IStatMod {
        public float Potency { get; }
        public StatType Type { get; private set; }

        public StatMod(float potency, StatType statType) {
            Potency = potency;
            Type = statType;
        }

        public void Apply(Status status) {
            status.ModifyStat(Type, Potency);
        }
    }
}
