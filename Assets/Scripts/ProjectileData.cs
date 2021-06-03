using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Data", menuName = "Projectile Data")]
public class ProjectileData : ScriptableObject{
    public float Speed;
    public ProjectileType type;
    public Damage.DamageTypeAndAmount[] damageTypesAndAmounts;

    public enum ProjectileType {
        Splash,
        Normal
    }
}
