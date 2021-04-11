using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : Projectile
{
    private bool alreadyHit = false;
    private Vector3 lastTargetPosition;

    private void Update()
    {
        MoveTowardsTarget();
    }

    protected override void MoveTowardsTarget()
    {
        //  Keep track of target's last position in case target dies before 
        //  projectile reaches it
        if (Target.gameObject.activeInHierarchy == true )
        {
            lastTargetPosition = Target.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, lastTargetPosition, Speed * Time.deltaTime);

        //  If target is dead and projectile reaches the target's last position
        if (transform.position == lastTargetPosition)
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
            if ((Target.gameObject.activeInHierarchy == true && collision.transform == Target) || Target == null)
            {
                DealDamage(enemy);
                //  Prevent multiple enemies from taking damage before the projectile gets destroyed
                alreadyHit = true;
                Destroy(gameObject);
            }
        }
    }
}
