using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : IEffect {
    public float Potency { get; }

    public Heal(float potency) {
        Potency = potency;
    }

    public void Apply(Status status) {
        status.RestoreHealth(Potency);
    }
}
