using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage{
    public enum DamageType {
        Kinetic,
        Fire,
        Poison,
        Lightning,
        Cold
    }

    [Serializable]
    public struct DamageTypeAndAmount {
        public DamageType type;
        public float amount;
    }

    public float CalculateDamage(Status unitStatus, DamageTypeAndAmount[] damageTypes) {
        float totalDamage = 0;
        for (int i = 0; i < damageTypes.Length; i++) {
            switch (damageTypes[i].type) {
                case DamageType.Kinetic:
                    totalDamage += (1 - unitStatus.Armor / 100) * damageTypes[i].amount;
                    break;
                case DamageType.Fire:
                    totalDamage += (1 - unitStatus.FireResist / 100) * damageTypes[i].amount;
                    break;
                case DamageType.Poison:
                    totalDamage += (1 - unitStatus.PoisonResist / 100) * damageTypes[i].amount;
                    break;
                case DamageType.Lightning:
                    totalDamage += (1 - unitStatus.LightningResist / 100) * damageTypes[i].amount;
                    break;
                case DamageType.Cold:
                    totalDamage += (1 - unitStatus.ColdResist / 100) * damageTypes[i].amount;
                    break;
                default:
                    throw new Exception("Damage type: " + damageTypes[i].type + " not implemented");
            }
        }
        return totalDamage;
    }
}
