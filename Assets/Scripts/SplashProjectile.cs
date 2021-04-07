using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashProjectile : Projectile
{
    public override Transform Target { get; set; }
    public override float Damage { get; set; }
    public override float Speed { get; set; }

    private List<Enemy> enemiesEffected;
    private bool triggered = false;
    private CircleCollider2D circle;

    private void Start()
    {
        enemiesEffected = new List<Enemy>();
        circle = GetComponent<CircleCollider2D>();
    }

    void Update()
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
        if (Target != null && Target.gameObject.activeInHierarchy && triggered == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);

            if (transform.position == Target.transform.position)
            {
                circle.enabled = true;
                StartCoroutine(ApplySlashDamage());
                triggered = true;
            }
        }
    }

    private IEnumerator ApplySlashDamage()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (Enemy enemy in enemiesEffected)
        {
            DealDamage(enemy);
        }

        Destroy(gameObject);
    }

    public override void DealDamage(Enemy enemy)
    {
        float radius = gameObject.GetComponent<CircleCollider2D>().radius;
        float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - enemy.transform.position.x, 2)
            + Mathf.Pow(transform.position.y - enemy.transform.position.y, 2));
        float damageDealt = radius / distance * Damage;

        //Debug.Log(string.Format("radius: {0}, distance: {1}, damage: {2}", radius, distance, damageDealt));

        enemy.TakeDamage(damageDealt);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesEffected.Add(enemy);
        }
    }
}
