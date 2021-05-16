using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected GameManager gameManager;
    protected SoundManager soundManager;
    protected MapManager mapManager;

    /// <summary>
    /// Used to select the appropriate sound for the collsion
    /// </summary>
    protected enum CollsionType
    {
        enemy,
        dirt,
        stone,
        explosion
    }

    public Transform Target { get; protected set; }
    public float Damage { get; protected set; }
    public float Speed { get; protected set; }
    public Vector3 LastTargetPosition { get; protected set; }
    protected abstract void DealDamage(Enemy enemy);
    
    protected abstract void MoveTowardsTarget();
    
    protected abstract void OnTriggerEnter2D(Collider2D collision);

    protected abstract void PlayCollsionAudio(CollsionType collsionType);
    
    public virtual void Initialize(Transform target, float damage, float speed)
    {
        gameManager = GameManager.Instance;
        soundManager = gameManager.SoundManager;
        mapManager = gameManager.MapManager;

        Target = target;
        Damage = damage;
        Speed = speed;
        LastTargetPosition = target.position;
    }
}
