public interface IEffect {
    /// <summary>
    /// The strength of the effect
    /// </summary>
    public float Potency { get; }

    /// <summary>
    /// The Unit's Status this effect applies to
    /// </summary>
    public Status UnitStatus { get; }

    /// <summary>
    /// Apply the effect to Unit
    /// </summary>
    public void Apply();
}