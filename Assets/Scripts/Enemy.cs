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

    public  delegate void EnemyReachedExit(Enemy enemy);
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
            if (transform.position != path[currentPathIndex])
            {
                transform.position = Vector3.MoveTowards(transform.position, path[currentPathIndex], Speed * Time.deltaTime);
            }
            else
            {
                currentPathIndex++;
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

    public void Spawn(Vector3 position)
    {
        //Debug.Log("Enemy spawn");
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        //Debug.Log("Enemy despawn");
        gameObject.SetActive(false);
    }
}
