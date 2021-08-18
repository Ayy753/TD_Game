namespace DefaultNamespace.EffectSystem {
    using DefaultNamespace.StatusSystem;

    public interface IStatMod : IEffect {
        public StatType Type { get; }
    }
}
