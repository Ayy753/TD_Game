namespace DefaultNamespace.EffectSystem {

    using DefaultNamespace.SoundSystem;
    using DefaultNamespace.StatusSystem;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public class OnEffectUsedEventArgs : EventArgs {
        public Vector3 Position { get; private set; }
        public SoundType SoundType { get; private set; }
        public float Radius { get; private set; }
        public string ParticleName { get; set; }

        public OnEffectUsedEventArgs(Vector3 position, SoundType soundType, float radius, string particleName) {
            Position = position;
            SoundType = soundType;
            Radius = radius;
            ParticleName = particleName;
        }
    }

    public class EffectGroup : ScriptableObject {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public float Radius { get; private set; }
        public TargetType Type { get; private set; }
        public string ParticleName { get; private set; }
        public SoundType SoundType { get; private set; }
        public float Cooldown { get; private set; }

        private IEffect[] Effects;
        private EffectableFinder effectableFinder;

        public static event EventHandler<OnEffectUsedEventArgs> OnEffectUsed;

        public enum TargetType {
            Individual, Area
        }

        private void OnEnable() {
            effectableFinder = GameObject.Find("EffectableFinder").GetComponent<EffectableFinder>();
        }

        public void Init(string name, string description, IEffect[] effects, TargetType targetType, string particleType, SoundType soundType, float radius, float cooldown) {
            Name = name;
            Description = description;
            Effects = effects;
            Type = targetType;
            Radius = radius;
            ParticleName = particleType;
            SoundType = soundType;
            Cooldown = cooldown;
        }

        public IEffect[] GetEffects() {
            //  IStatusEffect effects must be cloned
            IEffect[] effects = new IEffect[Effects.Length];
            for (int i = 0; i < Effects.Length; i++) {
                if (Effects[i] is IStatusEffect statusEffect) {
                    effects[i] = statusEffect.Clone();
                }
                else {
                    effects[i] = Effects[i];
                }
            }
            return effects;
        }

        public string GetEffectInfo() {
            StringBuilder result = new StringBuilder();

            foreach (IEffect effect in Effects) {
                if (effect is Damage damage) {
                    result.Append( string.Format("Deals {0} {1} damage\n", effect.Potency, damage.Type));
                }
                else if (effect is DamageOverTime dot) {
                    result.Append(string.Format("Deals {0} {1} damage over {2} seconds\n", effect.Potency, dot.Type, dot.Duration));
                }
                else if (effect is Debuff debuff) {
                    result.Append(string.Format("Reduces {0} by {1} for {2} seconds\n", debuff.Type, effect.Potency, debuff.Duration));
                }
                else if (effect is Buff buff) {
                    result.Append(string.Format("Increases {0} by {1} for {2} seconds\n", buff.Type, effect.Potency, buff.Duration));
                }
                else if (effect is Heal) {
                    result.Append(string.Format("Restores {0} HP\n", effect.Potency));
                }
            }
            return result.ToString();
        }

        public float GetTotalDamage() {
            float total = 0;
            foreach (IEffect effect in Effects) {
                if (effect is Damage damage) {
                    total += damage.Potency;
                }
                else if (effect is DamageOverTime dot) {
                    total += dot.Potency;
                }
            }
            return total;
        }

        public void EffectTarget(IEffectable target) {
            ApplyEffects(target);
            OnEffectUsed?.Invoke(null, new OnEffectUsedEventArgs(target.GetTransform().position, SoundType, Radius, ParticleName));
        }

        public void EffectTarget(IEffectable target, Vector3 impactPosition) {
            ApplyEffects(target);
            OnEffectUsed?.Invoke(null, new OnEffectUsedEventArgs(impactPosition, SoundType, Radius, ParticleName));
        }

        public void EffectArea(Vector3 position) {
            foreach (IEffectable target in GetEffectableObjectsInRange(position)) {
                ApplyEffects(target);
            }

            OnEffectUsed?.Invoke(null, new OnEffectUsedEventArgs(position, SoundType, Radius, ParticleName));
        }

        private List<IEffectable> GetEffectableObjectsInRange(Vector3 center) {
            return effectableFinder.GetEffectableObjectsInRange(center, Radius);
        }

        private void ApplyEffects(IEffectable target) {
            Status status = target.GetStatus();
            status.ApplyEffectGroup(this);
        }
    }
}
