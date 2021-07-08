using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stat {
    public virtual float Value { get { return baseValue + modification; } protected set{} }
    protected float baseValue;
    protected float modification;
    public virtual void ModifyStat(float amount) {
        modification += amount;
    }

    public virtual void Initialize() {
        modification = 0;
    }
}
