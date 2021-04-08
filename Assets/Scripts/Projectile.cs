using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public abstract Transform Target { get; set; }
    public abstract float Damage { get; set; }
    public abstract float Speed { get; set; }

    public abstract void Initialize(Transform target, float damage, float speed);

    protected abstract void DealDamage(Enemy enemy);

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    Enemy enemy = other.GetComponent<Enemy>();
    //    if (enemy != null)
    //    {
    //        enemy.TakeDamage(Damage);
    //    }
    //    Destroy(gameObject);
    //}
}
