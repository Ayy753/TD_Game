using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    //  Contains this tower's attributes
    private TowerData TowerData;
    [SerializeField]
    private GameObject projectilePrefab;

    DateTime TimeSinceLastShot = DateTime.MinValue;

    private void Start()
    {
        enemySpawner = GameManager.Instance.EnemySpawner;


        Debug.Log(Distance(Vector3.zero, new Vector3(10, 10, 0)));
    }

    private void Update()
    {
        ShootLogic();
    }

    private void ShootLogic()
    {
        if (DateTime.Now >= TimeSinceLastShot.AddSeconds(TowerData.ReloadTime) )
        {
            List<Enemy> enemies = enemySpawner.GetEnemies();
            Enemy closestEnemy = null;
            float shortestDistance = float.MaxValue;

            foreach (Enemy enemy in enemies)
            {
                if (enemy.isActiveAndEnabled)
                {
                    float distance = Distance(transform.position, enemy.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy != null && shortestDistance <= TowerData.Range)
            {
                Debug.Log("Shortest distance: " + shortestDistance);
                Projectile projectile = GameObject.Instantiate(projectilePrefab, transform.position, new Quaternion()).GetComponent<Projectile>();
                projectile.Initialize(closestEnemy.gameObject.transform, TowerData.Damage, 6f);
                TimeSinceLastShot = DateTime.Now;
            }
        }
    }

    private float Distance(Vector3 start, Vector3 finish)
    {
        return Mathf.Sqrt(Mathf.Pow(finish.x - start.x, 2f) + Mathf.Pow(finish.y - start.y, 2f));
    }

    public void SetTowerData(TowerData towerData)
    {
        TowerData = towerData;
    }
}
