using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resistance : Stat {
    public Resistance(float baseValue) {
        this.baseValue = baseValue;
    }
}

public class Speed : Stat {
    public override float Value {
        get{
            float effectiveValue = baseValue + modification;
            if (effectiveValue < 0)
                return 0;
            else
                return effectiveValue;
        }
    }

    public Speed(float baseValue) {
        this.baseValue = baseValue;
    }
}

public class Health : Stat {
    public float MaxHealth { get { return baseValue + modification; } }
    public override float Value { get { return MaxHealth - DamageInflicted; } }
    public float DamageInflicted {
        get { return damageInflicted; }
        protected set{
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
