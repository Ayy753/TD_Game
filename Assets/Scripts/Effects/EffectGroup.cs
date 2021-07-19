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


    public string GetEffectInfo() {
        string result = "";
        foreach (IEffect effect in Effects) {
            if (effect is Damage) {
                result += string.Format("Deals {0} {1} damage\n", effect.Potency, ((Damage)effect).Type);
            }
            else if (effect is DamageOverTime) {
                result += string.Format("Deals {0} {1} damage over {2} seconds\n", effect.Potency, ((DamageOverTime)effect).Type, ((DamageOverTime)effect).Duration);
            }
            else if (effect is Debuff) {
                result += string.Format("Reduces {0} by {1} for {2} seconds\n", ((Debuff)effect).Type, effect.Potency, ((Debuff)effect).Duration);
            }
            else if (effect is AreaDamage) {
                result += string.Format("Deals {0} {1} in a {2}m radius\n", effect.Potency, ((AreaDamage)effect).Type, ((AreaDamage)effect).Radius);
            }
            else if (effect is Buff) {
                result += string.Format("Increases {0} by {1} for {2} seconds\n", ((Buff)effect).Type, effect.Potency, ((Buff)effect).Duration);
            }
        }
        return result;
    }

    public float GetTotalDamage() {
        float total = 0;
        foreach (IEffect effect in Effects) {
            if (effect is Damage) {
                total += ((Damage)effect).Potency;
            }
            else if (effect is DamageOverTime) {
                total += ((DamageOverTime)effect).Potency;
            }
            else if (effect is AreaDamage) {
                total += ((AreaDamage)effect).Potency;
            }
        }
        return total;
    }
}
