namespace DefaultNamespace.EffectSystem {
    public interface IStatMod : IEffect {
        public Status.StatType Type { get; }
    }
}
