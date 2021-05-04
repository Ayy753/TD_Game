using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : Projectile
{
    private bool alreadyHit = false;
    private bool targetDead = false;

    private void Update()
    {
        MoveTowardsTarget();
    }
    private void OnEnable()
    {
        Enemy.OnEnemyDied += HandleEnemyDied;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyDied -= HandleEnemyDied;
    }
    
    /// <summary>
    /// Check if it was target that died
    /// </summary>
    /// <param name="enemy"></param>
    private void HandleEnemyDied(Enemy enemy)
    {
        if (enemy.transform == Target)
        {
            targetDead = true;
        }
    }
    
    /// <summary>
    /// Follows the target if its still alive
    /// if the target dies, projectile moves to enemy's last position
    /// </summary>
    protected override void MoveTowardsTarget()
    {
        //  Prevent projectile from targetting a dead enemy that got respawned
        if (targetDead == false)
        {
            LastTargetPosition = Target.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, LastTargetPosition, Speed * Time.deltaTime);

        //  If target is dead and projectile reaches the target's last position
        if (transform.position == LastTargetPosition)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Deals damage to the enemy 
    /// </summary>
    /// <param name="enemy"></param>
    protected override void DealDamage(Enemy enemy)
    {
        enemy.TakeDamage(Damage);
    }

    /// <summary>
    /// If the target is still alive, ignore collisions with other enemies
    /// otherwise deal damage to any other enemy hit
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null && alreadyHit == false)
        {
            //  Prevent projectile from hitting another enemy while target is still alive
            if ((targetDead == false && collision.transform == Target) || targetDead == true)
            {
                DealDamage(enemy);
                //  Prevent multiple enemies from taking damage before the projectile gets destroyed
                alreadyHit = true;
                //Destroy(gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Used by tower to set projectile's target, damage, and move speed
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    public override void Initialize(Transform target, float damage, float speed)
    {
        base.Initialize(target, damage, speed);
        alreadyHit = false;
        targetDead = false;
    }
}
