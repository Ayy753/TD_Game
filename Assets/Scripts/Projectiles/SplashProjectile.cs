using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashProjectile : Projectile, IUnitRangeDetection {

    private bool IsDetonating = false;
    private List<Enemy> enemiesInRange;
    private const float detonationTime = 0.75f;
    private const float splashRadius = 3f;

    private CircleCollider2D rangeCollider;

    private void Awake() {
        rangeCollider = transform.Find("RangeCollider").GetComponent<CircleCollider2D>();
    }

    private void Update() {
        if (IsDetonating == false && transform.position == LastTargetPosition) {
            StartCoroutine(DetonationCountdown());
        }
        else {
            MoveTowardsTarget();
        }
    }

    public override void Initialize(Vector3 startPos, Transform target) {
        base.Initialize(startPos, target);
        enemiesInRange = new List<Enemy>();
        LastTargetPosition = target.position;
        IsDetonating = false;

        rangeCollider.gameObject.SetActive(false);
    }

    protected override void MoveTowardsTarget() {
        transform.position = Vector3.MoveTowards(transform.position, LastTargetPosition, Time.deltaTime * ProjectileData.Speed);
    }

    private IEnumerator DetonationCountdown() {
        IsDetonating = true;
        rangeCollider.gameObject.SetActive(true);
        yield return new WaitForSeconds(detonationTime);
        Detonate();
    }

    private void Detonate() {
        Debug.Log("Enemies in range of splash: " + enemiesInRange.Count);

        foreach (Enemy enemy in enemiesInRange) {
            int numDamageTypes = ProjectileData.damageTypesAndAmounts.Length;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            float damageScale = ((splashRadius - distance) / splashRadius);

            Damage.DamageTypeAndAmount[] scaledDamageTypes = new Damage.DamageTypeAndAmount[numDamageTypes];

            //  Ensure enemy is still within range (if damage scale > 0)
            if (damageScale > 0) {
                for (int i = 0; i < numDamageTypes; i++) {
                    scaledDamageTypes[i].amount = ProjectileData.damageTypesAndAmounts[i].amount * damageScale;
                    scaledDamageTypes[i].type = ProjectileData.damageTypesAndAmounts[i].type;
                    Debug.Log(string.Format("Applying scaled damage Type: {0}, Amount: {1}", scaledDamageTypes[i].type, scaledDamageTypes[i].amount));
                }
                Damage.ApplyDamage(enemy, scaledDamageTypes);
            }
        }

        gameObject.SetActive(false);
    }

    public void UnitEnteredRange(IUnit unit) {
        Debug.Log("unit entered range");
        if (unit.GetType() == typeof(Enemy)) {
            enemiesInRange.Add((Enemy)unit);
        }
    }

    public void UnitLeftRange(IUnit unit) {
        Debug.Log("unit left range");
        if (unit.GetType() == typeof(Enemy)) {
            enemiesInRange.Remove((Enemy)unit);
        }
    }

    public float GetRange() {
        return splashRadius;
    }
}
