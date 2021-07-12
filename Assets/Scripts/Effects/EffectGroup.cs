using UnityEngine;

/// <summary>
/// Represents any collection of effects such as projectiles, buffs, debuffs, etc
/// </summary>
public class EffectGroup : ScriptableObject{
    public string Name { get; private set; }
    public string Description { get; private set; }
    private IEffect[] Effects;

    public void Init(string name, string description, IEffect[] effects) {
        Name = name;
        Description = description;
        Effects = effects;
    }

    /// <summary>
    /// Returns array of IEffects
    /// </summary>
    /// <returns></returns>
    public IEffect[] GetEffects() {
        //  IStatusEffect effects must be cloned
        IEffect[] effects = new IEffect[Effects.Length];
        for (int i = 0; i < Effects.Length; i++) {
            if (Effects[i] is IStatusEffect) {
                effects[i] = ((IStatusEffect)Effects[i]).Clone();
            }
            else {
                effects[i] = Effects[i];
            }
        }
        return effects;
    }
}
