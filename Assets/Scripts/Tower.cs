using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDisplayable
{
    [SerializeField]
    public TowerData TowerData;

    DateTime TimeSinceLastShot = DateTime.MinValue;
    Transform radiusIndicator;
    List<Enemy> enemiesInRange;

    private void Start()
    {
        radiusIndicator = transform.Find("RadiusIndicator");
        enemiesInRange = new List<Enemy>();
    }

    private void Update()
    {
        ShootLogic();
    }

    private void ShootLogic()
    {
        if (DateTime.Now >= TimeSinceLastShot.AddSeconds(TowerData.ReloadTime))
        {
            Enemy closestEnemy = null;
            float shortestDistance = float.MaxValue;

            foreach (Enemy enemy in enemiesInRange)
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

    /// <summary>
    /// Show the radius indicator
    /// </summary>
    private void OnMouseEnter()
    {
        radiusIndicator.gameObject.SetActive(true);
        radiusIndicator.localScale = new Vector3(TowerData.Range * 2, TowerData.Range * 2, 0);
    }

    /// <summary>
    /// Hide the radius indicator
    /// </summary>
    private void OnMouseExit()
    {
        radiusIndicator.gameObject.SetActive(false);
    }

    /// <summary>
    /// Used by range detector 
    /// </summary>
    /// <param name="enemy"></param>
    public void EnemyEnteredRange(Enemy enemy)
    {
        enemiesInRange.Add(enemy);   
    }

    /// <summary>
    /// Used by range detector
    /// </summary>
    /// <param name="enemy"></param>
    public void EnemyLeftRange(Enemy enemy)
    {
        enemiesInRange.Remove(enemy);
    }
}