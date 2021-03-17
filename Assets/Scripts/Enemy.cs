using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PathFinder pathFinder;
    private GameManager gameManager;
    private List<Vector3Int> path;
    private int currentPathIndex;

    [SerializeField]
    private float Speed = 10f;

    //  To compensate for the 0.5 unit offset of the tilemap system
    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0f);

    public delegate void EnemyReachedExit(Enemy enemy);
    public static event EnemyReachedExit OnEnemyReachedGate;

    private void OnEnable()
    {
        //Debug.Log("Enemy enable");
        gameManager = GameManager.Instance;
        pathFinder = gameManager.PathFinder;
        path = pathFinder.Path;
        currentPathIndex = 0;
    }

    /// <summary>
    /// Moves through path towards exit
    /// </summary>
    void Update()
    {
        if (currentPathIndex <= path.Count-1)
        {
            Vector3 target = path[currentPathIndex] + tilemapOffset;

            if (transform.position != target)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);
            }
            else
            {
                currentPathIndex++;

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
        else
        {
            if (OnEnemyReachedGate != null)
            {
                OnEnemyReachedGate.Invoke(this);
            }
            Despawn();
        }
    }

    /// <summary>
    /// Spawn this enemy into the world
    /// </summary>
    /// <param name="position"></param>
    public void Spawn(Vector3 position)
    {
        //Debug.Log("Enemy spawn");
        transform.position = position;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Despawn this enemy
    /// </summary>
    public void Despawn()
    {
        //Debug.Log("Enemy despawn");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the unit's speed
    /// </summary>
    /// <param name="newSpeed"></param>
    public void SetSpeed(float newSpeed)
    {
        Speed = newSpeed;
    }
}
