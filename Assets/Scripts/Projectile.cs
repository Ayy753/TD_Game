using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Transform Target { get; protected set; }
    public float Damage { get; protected set; }
    public float Speed { get; protected set; }
    public Vector3 LastTargetPosition { get; protected set; }
    protected abstract void DealDamage(Enemy enemy);
    
    protected abstract void MoveTowardsTarget();
    
    protected abstract void OnTriggerEnter2D(Collider2D collision);
    
    public virtual void Initialize(Transform target, float damage, float speed)
    {
        Target = target;
        Damage = damage;
        Speed = speed;
        LastTargetPosition = target.position;
    }
}
