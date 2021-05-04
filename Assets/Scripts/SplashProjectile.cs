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
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        //  Set particle Explode duration
        ParticleSystem.MainModule main = particle.main;
        main.startLifetime = ExplodeDuration;

        //  Scale speed in relation to radius
        main.startSpeed = SplashRadius * 100;
    }

    private void OnEnable()
    {
        //  Show projectile sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    private void OnDisable()
    {
        print("projectile disabled");
        circleCollider.enabled = false;
        StopCoroutine(ApplySlashDamage());
        triggered = false;

        //  Disable explosion particle effect
        transform.GetChild(0).gameObject.SetActive(false);
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
        //  Enable explosion particle effect
        transform.GetChild(0).gameObject.SetActive(true);

        //  Hide projectile sprite
        spriteRenderer.enabled = false;

        //  Enable circle collider and set it's radius to splash radius
        circleCollider.enabled = true;
        circleCollider.radius = SplashRadius;

        //  Wait for explosion effect to occur and allow for enemies to collide with collider
        yield return new WaitForSeconds(ExplodeDuration);

        //  Destroy this projectile
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Moves towards target's initial position at the time of projectile's creation
    /// </summary>
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

    /// <summary>
    /// Used to find all target's within projectile splash range after it reaches
    /// the target destination
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            DealDamage(enemy);
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
    }
}
