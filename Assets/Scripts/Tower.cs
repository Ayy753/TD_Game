using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Zenject;

public class Tower : MonoBehaviour, IUnitRangeDetection {
    [Inject] private ObjectPool objectPool;

    private List<Enemy> enemiesInRange = new List<Enemy>();
    private float timeSinceLastShot = float.MaxValue;
    private Enemy target;
    private Transform turret;

    [field: SerializeField] public TowerData TowerData { get; private set; }
    public TargetMode CurrentTargetMode { get; private set; }

    public enum TargetMode {
        Closest,
        Furthest,
        Random,
        LowestHealth,
        HighestHealth
    }

    private void Awake() {
        turret = transform.Find("Turret");
        StartCoroutine(TurretTracking());
        StartCoroutine(TargetFinder());
    }

    private void Update() {
        ShootLogicTick();
    }

    private void ShootLogicTick() {
        timeSinceLastShot += Time.deltaTime;

        if (target != null && timeSinceLastShot >= TowerData.ReloadTime) {
            //  Ensure turret aligns with projectile when it fires
            FaceTarget(target.transform);

            //soundManager.PlaySound(SoundManager.soundType.arrowRelease);

            //  Fire projectile
            Projectile projectile = objectPool.CreateProjectile(TowerData.projectileType);
            projectile.Initialize(transform.position, target.transform);

            timeSinceLastShot = 0;
        }
    }

    /// <summary>
    /// Rotates turret to face target
    /// </summary>
    /// <param name="target"></param>
    private void FaceTarget(Transform target) {
        Vector3 vectorDiff = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorDiff.y, vectorDiff.x) * Mathf.Rad2Deg;
        turret.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Rotates the turrent to face current target every 100ms
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurretTracking() {
        while (true) {
            if (target != null) {
                FaceTarget(target.transform);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Periodically searches for a new target
    /// </summary>
    /// <returns></returns>
    private IEnumerator TargetFinder() {
        while (true) {
            target = FindTarget();
            yield return new WaitForSeconds(0.33f);
        }
    }

    /// <summary>
    /// Selects the most suitable target based on the selected target mode
    /// </summary>
    /// <returns></returns>
    private Enemy FindTarget() {
        Enemy target = null;

        switch (CurrentTargetMode) {
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

    /// <summary>
    /// Selects the enemy closest to tower, within range
    /// </summary>
    /// <returns></returns>
    private Enemy FindClosestEnemy() {
        Enemy closestEnemy = null;
        float shortestDistance = float.MaxValue;

        foreach (Enemy enemy in enemiesInRange) {
            if (enemy.isActiveAndEnabled) {
                float distance = Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }

    /// <summary>
    /// Selects the enemy furthest from tower, within range
    /// </summary>
    /// <returns></returns>
    private Enemy FindFurthestEnemy() {
        Enemy closestEnemy = null;
        float furthestDistance = float.MinValue;

        foreach (Enemy enemy in enemiesInRange) {
            if (enemy.isActiveAndEnabled) {
                float distance = Distance(transform.position, enemy.transform.position);
                if (distance > furthestDistance) {
                    furthestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }

    /// <summary>
    /// Selects the enemy with the lowest health in range
    /// </summary>
    /// <returns></returns>
    private Enemy FindLowestHealthEnemy() {
        Enemy lowestEnemy = null;
        float lowestHealth = float.MaxValue;

        foreach (Enemy enemy in enemiesInRange) {
            if (enemy.isActiveAndEnabled) {
                float health = enemy.GetStatus().Health;
                if (health < lowestHealth) {
                    lowestHealth = health;
                    lowestEnemy = enemy;
                }
            }
        }
        return lowestEnemy;
    }

    /// <summary>
    /// Selects the enemy with the highest health in range
    /// </summary>
    /// <returns></returns>
    private Enemy FindHighestHealthEnemy() {
        Enemy highestEnemy = null;
        float highestHealth = float.MinValue;

        foreach (Enemy enemy in enemiesInRange) {
            if (enemy.isActiveAndEnabled) {
                float health = enemy.GetStatus().Health;
                if (health > highestHealth) {
                    highestHealth = health;
                    highestEnemy = enemy;
                }
            }
        }
        return highestEnemy;
    }

    /// <summary>
    /// Selects a random enemy within range
    /// </summary>
    /// <returns></returns>
    private Enemy FindRandomEnemy() {
        if (enemiesInRange.Count > 0) {
            int index = UnityEngine.Random.Range(0, enemiesInRange.Count);
            return enemiesInRange[index];
        }
        return null;
    }

    /// <summary>
    /// Calculates the distance between two points
    /// </summary>
    /// <param name="start"></param>
    /// <param name="finish"></param>
    /// <returns></returns>
    private float Distance(Vector3 start, Vector3 finish) {
        return Mathf.Sqrt(Mathf.Pow(finish.x - start.x, 2f) + Mathf.Pow(finish.y - start.y, 2f));
    }

    public void UnitEnteredRange(IUnit unit) {
        if (unit.GetType() == typeof(Enemy)) {
            enemiesInRange.Add((Enemy)unit);
        }
    }

    public void UnitLeftRange(IUnit unit) {
        if (unit.GetType() == typeof(Enemy)) {
            enemiesInRange.Remove((Enemy)unit);
        }
    }

    public float GetRange() {
        return TowerData.Range;
    }

    public class Factory : PlaceholderFactory<Tower> { }
}