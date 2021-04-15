using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDisplayable
{
    [SerializeField]
    public TowerData TowerData;

    float TimeSinceLastShot = float.MaxValue;
    Transform radiusIndicator;
    List<Enemy> enemiesInRange;
    Enemy Target;
    private Transform Turret;

    public TargetMode SelectedTargetMode { get; private set; } = TargetMode.Closest;
    public enum TargetMode
    {
        Closest, 
        Furthest,
        Random,
        LowestHealth,
        HighestHealth
    }

    private void Start()
    {
        radiusIndicator = transform.Find("RadiusIndicator");
        enemiesInRange = new List<Enemy>();
        Turret = transform.Find("Turret");

        StartCoroutine(TurretTracking());
    }

    private void Update()
    {
        ShootLogic();
    }

    private void OnMouseDown()
    {
        GameManager.Instance.GUIController.TargetTower(this);
    }

    private void ShootLogic()
    {
        Target = FindTarget();
        TimeSinceLastShot += Time.deltaTime;

        if (TimeSinceLastShot >= TowerData.ReloadTime && Target != null)
        { 
           if (Target != null)
            {
                //  Ensure turret aligns with projectile when it fires
                FaceTarget(Target.transform);

                //  Fire projectile
                Projectile projectile = GameObject.Instantiate(TowerData.ProjectilePrefab, transform.position, new Quaternion()).GetComponent<Projectile>();
                projectile.Initialize(Target.gameObject.transform, TowerData.Damage, 6f);
                //TimeSinceLastShot = DateTime.Now;
                TimeSinceLastShot = 0;
            }
        }
    }

    /// <summary>
    /// Rotates turret to face target
    /// </summary>
    /// <param name="target"></param>
    private void FaceTarget(Transform target)
    {
        Vector3 vectorDiff = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorDiff.y, vectorDiff.x) * Mathf.Rad2Deg;
        Turret.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Rotates the turrent to face current target every 100ms
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurretTracking()
    {
        while (true)
        {
            if (Target != null)
            {
                FaceTarget(Target.transform);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SelectTargetMode(TargetMode targetMode)
    {
        SelectedTargetMode = targetMode;
    }

    private Enemy FindTarget()
    {
        Enemy target = null;

        switch(SelectedTargetMode)
        {
            case TargetMode.Closest:
                target = FindClosestEnemy();
                break;
            case TargetMode.Furthest:
                target = FindFurthestEnemy();
                break;
            case TargetMode.HighestHealth:
                target = FindHighestHealthEnemy();
                break;
            case TargetMode.LowestHealth:
                target = FindLowestHealthEnemy();
                break;
            case TargetMode.Random:
                target = FindRandomEnemy();
                break;
            default:
                throw new Exception("Target mode invalid");
        }
        return target;
    }

    private Enemy FindClosestEnemy()
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
        return closestEnemy;
    }

    private Enemy FindFurthestEnemy()
    {
        Enemy closestEnemy = null;
        float furthestDistance = float.MinValue;

        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy.isActiveAndEnabled)
            {
                float distance = Distance(transform.position, enemy.transform.position);
                if (distance > furthestDistance)
                {
                    furthestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }

    private Enemy FindLowestHealthEnemy()
    {
        Enemy lowestEnemy = null;
        float lowestHealth = float.MaxValue;

        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy.isActiveAndEnabled)
            {
                float health = enemy.CurrentHealth;
                if (health < lowestHealth)
                {
                    lowestHealth = health;
                    lowestEnemy = enemy;
                }
            }
        }
        return lowestEnemy;
    }

    private Enemy FindHighestHealthEnemy()
    {
        Enemy highestEnemy = null;
        float highestHealth = float.MinValue;

        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy.isActiveAndEnabled)
            {
                float health = enemy.CurrentHealth;
                if (health > highestHealth)
                {
                    highestHealth = health;
                    highestEnemy = enemy;
                }
            }
        }
        return highestEnemy;
    }

    private Enemy FindRandomEnemy()
    {
        if (enemiesInRange.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, enemiesInRange.Count);
            return enemiesInRange[index];
        }
        return null;
    }

    private float Distance(Vector3 start, Vector3 finish)
    {
        return Mathf.Sqrt(Mathf.Pow(finish.x - start.x, 2f) + Mathf.Pow(finish.y - start.y, 2f));
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

    public string GetDisplayText()
    {
        return TowerData.ToString();
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