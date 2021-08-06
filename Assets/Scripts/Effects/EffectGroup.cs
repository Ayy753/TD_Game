using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents any collection of effects such as projectiles, buffs, debuffs, etc
/// </summary>
public class EffectGroup : ScriptableObject, IEffectableRangeDetection{
    private ParticlePool particlePool;

    public string Name { get; private set; }
    public string Description { get; private set; }
    public float Radius { get; private set; }
    public TargetType Type { get; private set; }
    public string ParticleName { get; private set; }
    private IEffect[] Effects;
    
    public enum TargetType {
        Individual, Area
    }

    private void OnEnable() {
        particlePool = GameObject.Find("ParticlePool").GetComponent<ParticlePool>();
    }

    public void Init(string name, string description, IEffect[] effects, TargetType targetType, string particleType, float radius = 0.25f ) {
        Name = name;
        Description = description;
        Effects = effects;
        Type = targetType;
        Radius = radius;
        ParticleName = particleType;
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
            else if (effect is Buff) {
                result += string.Format("Increases {0} by {1} for {2} seconds\n", ((Buff)effect).Type, effect.Potency, ((Buff)effect).Duration);
            }
            else if (effect is Heal) {
                result += string.Format("Restores {0} HP\n", effect.Potency);
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
        }
        return total;
    }

    public void EffectTarget(IEffectable target) {
        if (Type == TargetType.Area) {
            EffectArea(target.GetTransform().position);
        }
        else {
            ApplyEffectsToIndividual(target);
        }
    }

    public void EffectArea(Vector3 center) {
        if (Type == TargetType.Area) {
            ApplyEffectsInArea(center);
        }
    }

    private void ApplyEffectsInArea(Vector3 center) {
        List<IEffectable> effectableObjectsInRange = GetEffectableObjectsInRange(center);

        foreach (IEffectable effectable in effectableObjectsInRange) {
            ApplyEffectsToIndividual(effectable);
        }

        SpawnParticles(center);
    }

    private void ApplyEffectsToIndividual(IEffectable target) {
        Status status = target.GetStatus();
        status.ApplyEffectGroup(this);
    }

    private void SpawnParticles(Vector3 position) {
        particlePool.TryToSpawnParticleEffectAtPosition(ParticleName, position, Radius);
    }

    public List<IEffectable> GetEffectableObjectsInRange(Vector3 center) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, Radius);
        List<IEffectable> effectableObjectsInRange = new List<IEffectable>();

        foreach (var collider in colliders) {
            IEffectable effectable = collider.GetComponent<IEffectable>();
            if (effectable != null) {
                effectableObjectsInRange.Add(effectable);
            }
        }

        return effectableObjectsInRange;
    }
}
