using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : Projectile
{
    public override Transform Target { get; set; }
    public override float Damage { get; set; }
    public override float Speed { get; set; }
    
    private void Update()
    {
        TrackEnemy();
    }

    public override void Initialize(Transform target, float damage, float speed)
    {
        Target = target;
        Damage = damage;
        Speed = speed;
    }

    private void TrackEnemy()
    {
        if (Target != null && Target.gameObject.activeInHierarchy )
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
        }
    }

    public override void DealDamage(Enemy enemy)
    {
        enemy.TakeDamage(Damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            DealDamage(enemy);
        }

        Destroy(gameObject);
    }
}
