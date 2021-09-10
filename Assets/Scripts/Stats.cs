namespace DefaultNamespace.StatusSystem {

    public class Resistance : Stat {
        public Resistance(float baseValue) {
            this.baseValue = baseValue;
        }
    }

    public class Speed : Stat {
        private const float MINIMUM_SPEED = 0.2f;

        public Speed(float baseValue) {
            this.baseValue = baseValue;
            minimumValue = MINIMUM_SPEED;
        }
    }

    public class Health : Stat {
        public float MaxHealth { get { return baseValue + modification; } }
        public override float Value { get { return MaxHealth - DamageInflicted; } }
        public float DamageInflicted {
            get { return damageInflicted; }
            protected set {
                damageInflicted = value;
                if (damageInflicted < 0) {
                    damageInflicted = 0;
                }
            }
        }
        private float damageInflicted;

        public Health(float baseValue) {
            this.baseValue = baseValue;
        }

        public override void Initialize() {
            base.Initialize();
            damageInflicted = 0;
        }

        public void TakeDamage(float amount) {
            DamageInflicted += amount;
        }

        public void Heal(float amount) {
            DamageInflicted -= amount;
        }
    }
}