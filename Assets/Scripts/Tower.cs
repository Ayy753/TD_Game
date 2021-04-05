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

    private void Start()
    {
        enemySpawner = GameManager.Instance.EnemySpawner;
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (true)
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
                Projectile projectile = GameObject.Instantiate(projectilePrefab, transform.position, new Quaternion()).GetComponent<Projectile>();
                projectile.Initialize(closestEnemy.gameObject.transform, TowerData.Damage, 12f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private float Distance(Vector3 start, Vector3 finish)
    {
        return Mathf.Sqrt(Mathf.Pow(start.x - finish.x, 2) - Mathf.Pow(start.y - finish.y, 2));
    }

    public void SetTowerData(TowerData towerData)
    {
        TowerData = towerData;
    }
}
