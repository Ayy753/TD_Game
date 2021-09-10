namespace DefaultNamespace.StatusSystem {

    public abstract class Stat {

        protected float baseValue;
        protected float modification;
        protected float minimumValue = 0;
        protected float maximumValue = 100;

        public virtual float Value {
            get {
                float effectiveValue = baseValue + modification;
                if (effectiveValue < minimumValue)
                    return minimumValue;
                else if (effectiveValue > maximumValue) {
                    return maximumValue;
                }
                else
                    return effectiveValue;
            }
        }

        public virtual void ModifyStat(float amount) {
            modification += amount;
        }

        public virtual void Initialize() {
            modification = 0;
        }
    }
}
