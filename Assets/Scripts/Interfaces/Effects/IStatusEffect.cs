public interface IStatusEffect : IEffect {
    /// <summary>
    /// The initial duration of the effect
    /// </summary>
    public float Duration { get; }

    /// <summary>
    /// Remove the effect from Unit
    /// </summary>
    public void Remove();

    /// <summary>
    /// Executes every game tick
    /// </summary>
    public void OnTick();
}