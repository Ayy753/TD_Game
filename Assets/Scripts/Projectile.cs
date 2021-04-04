using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform Target { get; private set; }
    public float Damage { get; private set; }
    public float Speed { get; private set; }

    void Update()
    {
        if (Target != null && Target.gameObject.activeInHierarchy)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Transform target, float damage, float speed)
    {
        Target = target;
        Damage = damage;
        Speed = speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(Damage);
        }
        Destroy(gameObject);
    }

}
