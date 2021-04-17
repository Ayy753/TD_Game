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
            Destroy(gameObject);
        }
    }

    protected override void DealDamage(Enemy enemy)
    {
        enemy.TakeDamage(Damage);
    }

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
                Destroy(gameObject);
            }
        }
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
}
