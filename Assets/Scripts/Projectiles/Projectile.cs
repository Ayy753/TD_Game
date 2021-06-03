using UnityEngine;
using Zenject;

public abstract class Projectile : MonoBehaviour{
    public Transform Target { get; protected set; }
    public float Damage { get; protected set; }
    public float Speed { get; protected set; }
    public Vector3 LastTargetPosition { get; protected set; }

}
