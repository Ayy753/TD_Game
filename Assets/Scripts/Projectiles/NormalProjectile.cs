using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NormalProjectile : Projectile {

    public void Update() {
        MoveTowardsTarget();
    }

    public override void MoveTowardsTarget() {
        if (Target != null) {
            LastTargetPosition = Target.position;
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.deltaTime * ProjectileData.Speed);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, LastTargetPosition, Time.deltaTime * ProjectileData.Speed);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Enemy enemy = collision.GetComponentInChildren<Enemy>();

        //  If it hit an enemy
        if (enemy != null) {
            Damage.ApplyDamage(enemy, ProjectileData.damageTypesAndAmounts);
            gameObject.SetActive(false);
        }
    }

}
