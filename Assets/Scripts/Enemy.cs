using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDisplayable
{
    #region Variables
    private GameManager gameManager;
    private PathFinder pathFinder;

    private SpriteRenderer healthBarForeground;
    private Sprite Icon;

    private List<Vector3Int> path;
    private int currentPathIndex;

    //  To compensate for the 0.5 unit offset of the tilemap system
    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0f);
    #endregion

    public delegate void EnemyReachedExit(Enemy enemy);
    public delegate void EnemyDied(Enemy enemy);

    public static event EnemyReachedExit OnEnemyReachedGate;
    public static event EnemyDied OnEnemyDied;

    #region Properties
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public int Value { get; private set; } = 5;
    public string Name { get; private set; } = "Enemy";

    [SerializeField]
    private float Speed { get; set; } = 1f;
    #endregion

    private void Start()
    {
        healthBarForeground = transform.parent.Find("HealthbarFront").GetComponent<SpriteRenderer>();
        Icon = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    #region Methods
    private void OnEnable()
    {
        //Debug.Log("Enemy enable");
        gameManager = GameManager.Instance;
        pathFinder = gameManager.PathFinder;
        currentPathIndex = 0;
        path = pathFinder.CurrentPath;

        PathFinder.OnPathRecalculated += HandlePathRecalculated;
    }

    private void OnDisable()
    {
        PathFinder.OnPathRecalculated -= HandlePathRecalculated;
    }

    private void Update()
    {
        if (path != null)
        {
            if (currentPathIndex <= path.Count - 1)
            {
                Vector3 target = path[currentPathIndex] + tilemapOffset;

                if (transform.parent.position != target)
                {
                    transform.parent.position = Vector3.MoveTowards(transform.parent.position, target, Speed * Time.deltaTime);
                }
                else
                {
                    currentPathIndex++;

                    if (currentPathIndex < path.Count)
                    {
                        //  Rotate unit to face direction of next tile in path
                        Vector3 posNoOffset = transform.position - tilemapOffset;
                        if (path[currentPathIndex].x < posNoOffset.x)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 180);
                        }
                        else if (path[currentPathIndex].y < posNoOffset.y)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, -90);
                        }
                        else if (path[currentPathIndex].y > posNoOffset.y)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 90);
                        }
                        if (path[currentPathIndex].x > posNoOffset.x)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                    }
                }
            }
            else
            {
                if (OnEnemyReachedGate != null)
                {
                    OnEnemyReachedGate.Invoke(this);
                }

                Despawn();
            }
        }
    }

    /// <summary>
    /// Responds to path change event
    /// </summary>
    /// <param name="newPath"></param>
    private void HandlePathRecalculated(List<Vector3Int> newPath)
    {
        path = newPath;
    }

    /// <summary>
    /// Spawn this enemy into the world
    /// </summary>
    /// <param name="position"></param>
    public void Spawn(Vector3 position, float maxHealth)
    {
        transform.parent.position = position;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        transform.parent.gameObject.SetActive(true);
    }

    /// <summary>
    /// Despawn this enemy
    /// </summary>
    public void Despawn()
    {
        transform.parent.gameObject.SetActive(false);
        //  Reset healthbar
        healthBarForeground.gameObject.transform.localScale = new Vector3(1, 0.25f, 1);
    }

    /// <summary>
    /// Sets the unit's speed
    /// </summary>
    /// <param name="newSpeed"></param>
    public void SetSpeed(float newSpeed)
    {
        Speed = newSpeed;
    }

    /// <summary>
    /// Used to deal damage to this enemy
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        float healthPercent = CurrentHealth / MaxHealth;

        //  Check if hit was a killing blow
        if (CurrentHealth <= 0)
        {
            if (OnEnemyDied != null)
            {
                OnEnemyDied.Invoke(this);
            }

            Despawn();
        }
        else
        {
            healthBarForeground.gameObject.transform.localScale = new Vector3(healthPercent, 0.25f, 1);
        }

    }

    public string GetDisplayText()
    {
        return string.Format("Name:{0}\nHealth:{1}/{2}\nValue:{3}", Name, CurrentHealth, MaxHealth, Value);
    }

    #endregion
}
