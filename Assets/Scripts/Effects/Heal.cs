namespace DefaultNamespace.EffectSystem {
    public class Heal : IEffect {
        public float Potency { get; }

        public Heal(float potency) {
            Potency = potency;
        }

        public void Apply(Status status) {
            status.RestoreHealth(Potency);
        }
    }
}