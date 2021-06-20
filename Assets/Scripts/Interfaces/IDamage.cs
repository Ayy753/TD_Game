public interface IDamage : IEffect {
    public DamageType Type { get; }

    public enum DamageType {
        Physical, Cold, Fire, Poison, Lightning, Max
    }

    /// <summary>
    /// Calculates the amount of damage done to the unit based on the damage type/potency and the unit's resistance to that type
    /// </summary>
    public float CalculateDamage();
}