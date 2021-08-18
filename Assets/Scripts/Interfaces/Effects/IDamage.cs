namespace DefaultNamespace.EffectSystem {
    
    using DefaultNamespace.StatusSystem;

    public enum DamageType {
        Physical, Cold, Fire, Poison, Lightning, Max
    }

    public interface IDamage : IEffect {
        public DamageType Type { get; }
        public float CalculateEffectiveDamage(Status effectableStatus);
    }
}
