using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : IDamage, IEffectableRangeDetection {
    public float Radius { get; }
    public float Potency { get; }
    public IDamage.DamageType Type { get; private set; }

    public AreaDamage(float potency, IDamage.DamageType damageType, float radius) {
        Potency = potency;
        Type = damageType;
        Radius = radius;
    }

    public List<IEffectable> GetEffectableObjectsInRange(Vector3 center) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, Radius);
        List<IEffectable> EffectableObjectsInRange = new List<IEffectable>();

        foreach (var collider in colliders) {
            IEffectable effectable = collider.GetComponent<IEffectable>();
            if (effectable != null) {
                EffectableObjectsInRange.Add(effectable);
            }
        }

        return EffectableObjectsInRange;
    }

    public void Apply(Status status) {
        Vector3 center = status.transform.position;
        List<IEffectable> effectableObjects = GetEffectableObjectsInRange(center);

        foreach (IEffectable effectable in effectableObjects) {
            float distanceRatio = CalculateDistanceRatio(center, effectable.GetTransform().position, Radius);
            float damage = CalculateDamage(effectable.GetStatus());
            float effectiveDamage = distanceRatio * damage;
            effectable.GetStatus().TakeDamage(effectiveDamage);
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
