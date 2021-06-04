using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashProjectile : Projectile, IUnitRangeDetection {

    private bool IsDetonating = false;
    private const float detonationTime = 0.75f;
    private const float splashRadius = 3f;

    private CircleCollider2D rangeCollider;
    private ParticleSystem particles;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        rangeCollider = transform.Find("RangeCollider").GetComponent<CircleCollider2D>();
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        particles = transform.GetChild(0).GetComponent<ParticleSystem>();

        //  Set particle Explode duration
        ParticleSystem.MainModule main = particles.main;
        main.startLifetime = detonationTime;

        //  Scale speed in relation to radius
        main.startSpeed = splashRadius * 100;
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
        LastTargetPosition = target.position;
        IsDetonating = false;

        particles.gameObject.SetActive(false);
        rangeCollider.gameObject.SetActive(false);
        spriteRenderer.enabled = true;
    }

    protected override void MoveTowardsTarget() {
        transform.position = Vector3.MoveTowards(transform.position, LastTargetPosition, Time.deltaTime * ProjectileData.Speed);
    }

    private IEnumerator DetonationCountdown() {
        IsDetonating = true;
        rangeCollider.gameObject.SetActive(true);
        particles.gameObject.SetActive(true);
        spriteRenderer.enabled = false;
        
        yield return new WaitForSeconds(detonationTime);
        gameObject.SetActive(false);
    }


    private void ApplySplashDamage(Enemy enemy) {
        int numDamageTypes = ProjectileData.damageTypesAndAmounts.Length;
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        float damageScale = ((splashRadius - distance) / splashRadius);

        Damage.DamageTypeAndAmount[] scaledDamageTypes = new Damage.DamageTypeAndAmount[numDamageTypes];

        //  Ensure enemy is still within range (if damage scale > 0)
        if (damageScale > 0) {
            for (int i = 0; i < numDamageTypes; i++) {
                scaledDamageTypes[i].amount = ProjectileData.damageTypesAndAmounts[i].amount * damageScale;
                scaledDamageTypes[i].type = ProjectileData.damageTypesAndAmounts[i].type;
            }
            Damage.ApplyDamage(enemy, scaledDamageTypes);
        }
    }

    public void UnitEnteredRange(IUnit unit) {
        Debug.Log("unit entered range");
        if (unit.GetType() == typeof(Enemy)) {
            ApplySplashDamage((Enemy)unit);
        }
    }

    public void UnitLeftRange(IUnit unit) {
    }

    public float GetRange() {
        return splashRadius;
    }
}
