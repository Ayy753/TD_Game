using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Transform Target { get; set; }
    public float Damage { get; set; }
    public float Speed { get; set; }

    public virtual void Initialize(Transform target, float damage, float speed)
    {
        Target = target;
        Damage = damage;
        Speed = speed;
    }
    protected abstract void DealDamage(Enemy enemy);
    protected abstract void MoveTowardsTarget();
    protected abstract void OnTriggerEnter2D(Collider2D collision);
}
