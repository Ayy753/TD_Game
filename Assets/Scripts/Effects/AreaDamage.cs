using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : IDamage, IUnitRangeDetection {
    public float Radius { get; }
    public float Potency { get; }
    public IDamage.DamageType Type { get; private set; }

    public AreaDamage(float potency, IDamage.DamageType damageType, float radius) {
        Potency = potency;
        Type = damageType;
        Radius = radius;
    }

    public List<IUnit> GetUnitsInRange(Vector3 center) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, Radius);
        List<IUnit> enemiesInRange = new List<IUnit>();

        foreach (var collider in colliders) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) {
                enemiesInRange.Add(enemy);
            }
        }

        return enemiesInRange;
    }

    public void Apply(Status status) {
        Vector3 center = status.transform.position;
        List<IUnit> enemies = GetUnitsInRange(center);

        foreach (Enemy enemy in enemies) {
            float distanceRatio = CalculateDistanceRatio(center, enemy.transform.position, Radius);
            float damage = CalculateDamage(enemy.GetStatus());
            float effectiveDamage = distanceRatio * damage;
            enemy.GetStatus().TakeDamage(effectiveDamage);
        }
    }

    public float CalculateDamage(Status unitStatus) {
        float resistence = unitStatus.GetStat((Status.StatType)Type).Value;
        float effectiveDamage = (1 - resistence / 100) * Potency;
        return effectiveDamage;
    }

    public float CalculateDistanceRatio(Vector3 effectCenter, Vector3 unitPos, float radius) {
        float distance = Vector3.Distance(effectCenter, unitPos);
        float distanceRatio = (radius - distance) / radius;

        return distanceRatio;
    }
}
