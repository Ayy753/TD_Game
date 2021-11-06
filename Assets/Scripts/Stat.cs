using System;

namespace DefaultNamespace.StatusSystem {

    public abstract class Stat {

        protected float baseValue;
        protected float modification;
        protected float minimumValue = 0;
        protected float maximumValue = 100;
        protected float value;

        public virtual float Value {
            get {
                return value;
            }
        }

        public virtual float BaseValue {
            get {
                return baseValue;
            }
        }

        public virtual void ModifyStat(float amount) {
            modification += amount;
            CalculateValues();
        }

        protected virtual void CalculateValues() {
            float effectiveValue = baseValue + modification;
            if (effectiveValue < minimumValue)
                value = minimumValue;
            else if (effectiveValue > maximumValue) {
                value = maximumValue;
            }
            else
                value = effectiveValue;
        }

        public virtual void Initialize() {
            modification = 0;
            CalculateValues();
        }
    }
}
