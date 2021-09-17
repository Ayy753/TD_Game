namespace DefaultNamespace.StatusSystem {

    public class Resistance : Stat {
        public Resistance(float baseValue){
            this.baseValue = baseValue;
            minimumValue = -75f;
            Initialize();
        }
    }

    public class Armor : Resistance {
        public Armor(float baseValue) : base(baseValue) {
            minimumValue = 0f;
            Initialize();
        }
    }

    public class Speed : Stat {
        private const float MINIMUM_SPEED = 0.2f;

        public Speed(float baseValue){
            this.baseValue = baseValue;
            minimumValue = MINIMUM_SPEED;
            Initialize();
        }
    }

    public class Health : Stat {

        private float maxHealth, damageInflicted;

        public float MaxHealth { get { return maxHealth; } }
        public override float Value { get { return value; } }

        public float DamageInflicted {
            get { return damageInflicted; }
        }

        public Health(float baseValue){
            this.baseValue = baseValue;
            Initialize();
        }

        public override void Initialize() {
            damageInflicted = 0;
            base.Initialize();
        }

        protected override void CalculateValues() {
            if (damageInflicted < 0) {
                damageInflicted = 0;
            }

            maxHealth = baseValue + modification;
            value = MaxHealth - DamageInflicted;
        }

        public void TakeDamage(float amount) {
            damageInflicted += amount;
            CalculateValues();
        }

        public void Heal(float amount) {
            damageInflicted -= amount;
            CalculateValues();
        }
    }
}