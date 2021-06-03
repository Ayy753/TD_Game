using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (enemy != null) {
            Debug.Log("Projectile hit enemy");
            //enemy.Status.TakeDamage(ProjectileData.damageTypesAndAmounts);
            Damage.ApplyDamage(enemy, ProjectileData.damageTypesAndAmounts);
        }

        Destroy(gameObject);
    }
}
