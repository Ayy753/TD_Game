using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Data", menuName = "Projectile Data")]
public class ProjectileData : ScriptableObject{
    public float Speed;
    public ProjectileType type;
    //public Damage.DamageTypeAndAmount[] damageTypesAndAmounts;

    public enum ProjectileType {
        Splash,
        Normal,
        Sniper
    }

    /// <summary>
    /// Should only be used to display raw total damage.
    /// Actual damage inflicted should be calculated through the Damage class
    /// as unit's resistances impact the final value
    /// </summary>
    /// <returns></returns>
    public string RawTotalDamage() {
        float total = 0;

        //foreach (Damage.DamageTypeAndAmount damageType in damageTypesAndAmounts) {
        //    total += damageType.amount;
        //}

        return total.ToString();
    }
}
