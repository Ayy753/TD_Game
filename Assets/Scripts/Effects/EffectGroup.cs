namespace DefaultNamespace.EffectSystem {

    using DefaultNamespace.SoundSystem;
    using DefaultNamespace.StatusSystem;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class EffectGroup : ScriptableObject {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public float Radius { get; private set; }
        public TargetType Type { get; private set; }
        public string ParticleName { get; private set; }
        public SoundManager.SoundType SoundType { get; private set; }

        private IEffect[] Effects;
        private EffectableFinder effectableFinder;

        public static event EventHandler<OnEffectUsedEventArg> OnEffectUsed;

        public class OnEffectUsedEventArg : EventArgs {
            public Vector3 position;
        }

        public enum TargetType {
            Individual, Area
        }

        private void OnEnable() {
            effectableFinder = GameObject.Find("EffectableFinder").GetComponent<EffectableFinder>();
        }

        public void Init(string name, string description, IEffect[] effects, TargetType targetType, string particleType, SoundManager.SoundType soundType, float radius) {
            Name = name;
            Description = description;
            Effects = effects;
            Type = targetType;
            Radius = radius;
            ParticleName = particleType;
            SoundType = soundType;
        }

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
            ApplyEffects(target);
            OnEffectUsed?.Invoke(this, new OnEffectUsedEventArg { position = target.GetTransform().position });
        }

        public void EffectTarget(IEffectable target, Vector3 impactPosition) {
            ApplyEffects(target);
            OnEffectUsed?.Invoke(this, new OnEffectUsedEventArg { position = impactPosition });
        }

        public void EffectArea(Vector3 position) {
            foreach (IEffectable target in GetEffectableObjectsInRange(position)) {
                ApplyEffects(target);
            }

            OnEffectUsed?.Invoke(this, new OnEffectUsedEventArg { position = position });
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
