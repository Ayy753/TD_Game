using System;
using UnityEngine;
using Zenject;

public abstract class Projectile : MonoBehaviour{
    protected Transform Target { get; set; }
    protected Vector3 LastTargetPosition { get; set; }
    [field: SerializeField] public ProjectileData ProjectileData { get; protected set; }
    protected abstract void MoveTowardsTarget();
    public virtual void Initialize(Vector3 startPos, Transform target) {
        transform.position = startPos;
        Target = target;
    }

    public class Factory : PlaceholderFactory<ProjectileData.ProjectileType, Projectile> { }

}
