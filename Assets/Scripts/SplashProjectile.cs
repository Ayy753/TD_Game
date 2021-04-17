using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashProjectile : Projectile
{
    private float SplashRadius { get; set; } = 3f;
    private float ExplodeDuration { get; set; } = 0.5f;
    private bool triggered = false;
    private CircleCollider2D circleCollider;
    private ParticleSystem particle;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    /// <summary>
    /// A explosion that deals splash damage around a radius
    /// Anything that touches the collider during the explosion effect takes damage
    /// relative to it's distance from center
    /// </summary>
    /// <returns>A reference to the coroutine</returns>
    private IEnumerator ApplySlashDamage()
    {
        //  Set particle Explode duraction
        ParticleSystem.MainModule main = particle.main;
        main.startLifetime = ExplodeDuration;

        //  Scale speed in relation to radius
        main.startSpeed = SplashRadius * 100;

        //  Enable explosion particle effect
        transform.GetChild(0).gameObject.SetActive(true);

        //  Hide projectile sprite
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        //  Enable circle collider and set it's radius to splash radius
        circleCollider.enabled = true;
        circleCollider.radius = SplashRadius;

        //  Wait for explosion effect to occur and allow for enemies to collide with collider
        yield return new WaitForSeconds(ExplodeDuration);

        //  Destroy this projectile
        Destroy(gameObject);
    }
    
    protected override void MoveTowardsTarget()
    {
        if (triggered == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, LastTargetPosition, Speed * Time.deltaTime);

            if (transform.position == LastTargetPosition)
            {
                circleCollider.enabled = true;
                StartCoroutine(ApplySlashDamage());
                triggered = true;
            }
        }
    }

    /// <summary>
    /// Deals damage based on the distance from the explosion's center
    /// </summary>
    /// <param name="enemy"></param>
    protected override void DealDamage(Enemy enemy)
    {
        float radius = gameObject.GetComponent<CircleCollider2D>().radius;
        float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - enemy.transform.position.x, 2)
            + Mathf.Pow(transform.position.y - enemy.transform.position.y, 2));

        if (distance <= radius)
        {
            float damageDealt = damageDealt = ((radius - distance) / radius) * Damage;
            enemy.TakeDamage(damageDealt);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            DealDamage(enemy);
        }
    }
    
    public override void Initialize(Transform target, float damage, float speed)
    {
        base.Initialize(target, damage, speed);
    }
}
