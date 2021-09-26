namespace DefaultNamespace.EffectSystem {
    public interface IStatusEffect : IEffect {
        public int RemainingTicks { get; }
        public bool Expires { get; }
        public float Duration { get; }
        public void Remove();
        public void OnTick();
        public IStatusEffect Clone();
    }
}
