namespace DefaultNamespace.EffectSystem {
    using DefaultNamespace.StatusSystem;

    public class Damage : IDamage {
        public float Potency { get; }
        public DamageType Type { get; private set; }

        public Damage(float potency, DamageType damageType) {
            Potency = potency;
            Type = damageType;
        }

        public void Apply(Status status) {
            //  Total effect damage is applied by Status 
        }

        public float CalculateEffectiveDamage(Status effectableStatus) {
            float resistence = effectableStatus.GetStat((StatType)Type).Value;
            float effectiveDamage = (1 - resistence / 100) * Potency;
            return effectiveDamage;
        }
    }
}