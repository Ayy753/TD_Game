using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NormalProjectile : Projectile {
    private bool targetDied;
    private bool dealtDamage;

    private void Update() {
        MoveTowardsTarget();
    }

    public override void Initialize(Vector3 startPos, Transform target) {
        base.Initialize(startPos, target);
        targetDied = false;
        dealtDamage = false;
    }

    protected override void MoveTowardsTarget() {
        if (Target.gameObject.activeInHierarchy == false) {
            //  Make sure projectile doesn't start going after an enemy that respawned after dying
            targetDied = true;
        }

        if (targetDied != true) {
            LastTargetPosition = Target.position;
            transform.position = Vector3.MoveTowards(transform.position, Target.position, ProjectileData.Speed * Time.deltaTime);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, LastTargetPosition, ProjectileData.Speed * Time.deltaTime);
            if (transform.position == LastTargetPosition)
                gameObject.SetActive(false);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        Unit unit = collision.GetComponentInChildren<Unit>();

        //  If it hit a unit, and hasn't damaged anything yet (to prevent it from damaging multiple units at the same time)
        if (unit != null && dealtDamage == false) {

            foreach (var item in ProjectileData.damageTypesAndAmounts) {
                Debug.Log(string.Format("type: {0} amount: {1}", item.type, item.amount));
            }

            //  If the unit is the target
            if (unit.GetTransform() == Target) {
                dealtDamage = true;
                unit.ApplyDamage(ProjectileData.damageTypesAndAmounts);
                gameObject.SetActive(false);
            }
            //  If target is already dead, it can damage another unit
            else if (targetDied == true) {
                unit.ApplyDamage(ProjectileData.damageTypesAndAmounts);
                dealtDamage = true;
                gameObject.SetActive(false);
            }
        }
    }
}
