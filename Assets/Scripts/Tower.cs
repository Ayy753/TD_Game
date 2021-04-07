using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDisplayable
{
    private EnemySpawner enemySpawner;
    [SerializeField]
    private TowerData TowerData;

    DateTime TimeSinceLastShot = DateTime.MinValue;

    private void Start()
    {
        enemySpawner = GameManager.Instance.EnemySpawner;
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
                Projectile projectile = GameObject.Instantiate(TowerData.ProjectilePrefab, transform.position, new Quaternion()).GetComponent<Projectile>();
                projectile.Initialize(closestEnemy.gameObject.transform, TowerData.Damage, 6f);
                TimeSinceLastShot = DateTime.Now;
            }
        }
    }

    private float Distance(Vector3 start, Vector3 finish)
    {
        return Mathf.Sqrt(Mathf.Pow(finish.x - start.x, 2f) + Mathf.Pow(finish.y - start.y, 2f));
    }

    public string GetDisplayText()
    {
        return TowerData.ToString();
    }
}
