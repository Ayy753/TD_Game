public interface IStatusEffect : IEffect {
    /// <summary>
    /// The remaining duration of the effect
    /// </summary>
    public float RemainingDuration { get; }

    /// <summary>
    /// Remove the effect from Unit
    /// </summary>
    public void Remove();

    /// <summary>
    /// Executes every game tick
    /// </summary>
    public void OnTick();
}