namespace DefaultNamespace.EffectSystem {

    using DefaultNamespace.StatusSystem;

    public interface IEffect {
        /// <summary>
        /// The strength of the effect
        /// </summary>
        public float Potency { get; }

        /// <summary>
        /// Apply the effect to Unit
        /// </summary>
        public void Apply(Status status);
    }
}
