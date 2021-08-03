using System;

public interface Itargetable : ITransform{

    /// <summary>
    /// Fires when target is destroyed
    /// </summary>
    public event EventHandler TargetDisabled;

    public string GetName();
    public string GetDescription();
}
