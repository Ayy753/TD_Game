using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDisplayable
{
    #region Variables
    private GameManager gameManager;
    private PathFinder pathFinder;
    private MapManager mapManager;
    private SpriteRenderer healthBarForeground;
    private Sprite Icon;

    private List<Vector3Int> currentPath;
    private int currentPathIndex;
    private bool onMainPath = true;
    private List<Vector3Int> routeToPath;
    private int routeIndex;

    //  To compensate for the 0.5 unit offset of the tilemap system
    private static Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0f);

    //  The last tile walked on
    private Vector3Int lastTile = Vector3Int.down;
    //  Walk speed on current tile type
    private float walkSpeed;
    #endregion

    public delegate void EnemyReachedExit(Enemy enemy);
    public delegate void EnemyDied(Enemy enemy);
    public delegate void EnemyHit(Enemy enemy, float damage);

    public static event EnemyReachedExit OnEnemyReachedGate;
    public static event EnemyDied OnEnemyDied;
    public static event EnemyHit OnEnemyHit;

    #region Properties
    public float CurrentHealth { get; private set; }
    [SerializeField]
    public EnemyData EnemyData;
    #endregion

    private void Start()
    {
        healthBarForeground = transform.parent.Find("HealthbarFront").GetComponent<SpriteRenderer>();
        Icon = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    #region Methods
    private void OnEnable()
    {
        onMainPath = true;
        currentPathIndex = 0;

        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
            pathFinder = gameManager.PathFinder;
            mapManager = gameManager.MapManager;
            currentPath = pathFinder.CurrentPath;
        }

        FaceNextNode(currentPath[currentPathIndex]);
        PathFinder.OnPathRecalculated += HandlePathRecalculated;
        MapManager.OnStructureChanged += HandleStructureChanged;
    }

    private void OnDisable()
    {
        PathFinder.OnPathRecalculated -= HandlePathRecalculated;
        MapManager.OnStructureChanged -= HandleStructureChanged;
    }

    /// <summary>
    /// Individually handle the case where the sturcture map changes, but the
    /// main path doesnt need to be recalculated, and the enemy's path towards 
    /// the main path got blocked
    /// </summary>
    /// <param name="demolish"></param>
    private void HandleStructureChanged(bool demolish)
    {
        if (onMainPath == false)
        {
            if (pathFinder.PathBlocked(routeToPath))
            {
                (List<Vector3Int>, int) result = pathFinder.RouteToPath(Vector3Int.FloorToInt(transform.position));

                //  Route to new path
                routeToPath = result.Item1;

                //  Current index in route to path
                routeIndex = 0;

                //  Where enemy will be when it joins main path
                currentPathIndex = result.Item2;
            }
        }
    }

    private void Update()
    {
        if (Vector3Int.FloorToInt(transform.position) != lastTile)
        {
            lastTile = Vector3Int.FloorToInt(transform.position);
            walkSpeed = EnemyData.Speed - mapManager.GetTileCost(lastTile) / 15;
        }

        if (onMainPath)
        {
            transform.parent.position = Vector3.MoveTowards(transform.position, currentPath[currentPathIndex] + tilemapOffset, walkSpeed * Time.deltaTime);
            if (currentPath[currentPathIndex] + tilemapOffset == transform.position)
            {
                currentPathIndex++;
                if (currentPathIndex == currentPath.Count)
                {
                    OnEnemyReachedGate.Invoke(this);
                    Despawn();
                }
                else
                {
                    FaceNextNode(currentPath[currentPathIndex]);
                }
            }
        }
        else
        {
            transform.parent.position = Vector3.MoveTowards(transform.position, routeToPath[routeIndex] + tilemapOffset, walkSpeed * Time.deltaTime);
            if (routeToPath[routeIndex] + tilemapOffset == transform.position)
            {
                routeIndex++;
                if (routeIndex == routeToPath.Count)
                {
                    onMainPath = true;
                }
                else
                {
                    FaceNextNode(routeToPath[routeIndex]);
                }
            }
        }
    }

    /// <summary>
    /// Faces next path node
    /// </summary>
    private void FaceNextNode(Vector3Int nextNodePos)
    {
        //  Rotate unit to face direction of next tile in path
        Vector3 posNoOffset = transform.position - tilemapOffset;
        if (nextNodePos.x < posNoOffset.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (nextNodePos.y < posNoOffset.y)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (nextNodePos.y > posNoOffset.y)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (nextNodePos.x > posNoOffset.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
        
    /// <summary>
    /// Responds to path change event
    /// </summary>
    /// <param name="newPath"></param>
    private void HandlePathRecalculated(List<Vector3Int> newPath)
    {
        currentPath = newPath;

        (List<Vector3Int>, int) result = pathFinder.RouteToPath(Vector3Int.FloorToInt(transform.position));

        //  Route to new path
        routeToPath = result.Item1;

        //  Current index in route to path
        routeIndex = 0;

        //  Where enemy will be when it joins main path
        currentPathIndex = result.Item2;

        onMainPath = false;
    }

    /// <summary>
    /// Spawn this enemy into the world
    /// </summary>
    /// <param name="position"></param>
    public void Spawn(Vector3 position)
    {
        transform.parent.position = position;
        CurrentHealth = EnemyData.MaxHealth;
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
    /// Used to deal damage to this enemy
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        float healthPercent = CurrentHealth / EnemyData.MaxHealth;

        if (OnEnemyHit != null)
        {
            OnEnemyHit.Invoke(this, damage);
        }

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
        return string.Format("Name:{0}\nHealth:{1}/{2}\nValue:{3}\nDescription:{4}", EnemyData.Name, Mathf.RoundToInt(CurrentHealth), EnemyData.MaxHealth, EnemyData.Value, EnemyData.Description);
    }

    #endregion
}
