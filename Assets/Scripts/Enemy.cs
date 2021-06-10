using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, IUnit {
    [Inject] private readonly IPathfinder pathFinder;
    [SerializeField] public EnemyData enemyData;
    private IUnitInput unitInput;
    private IUnitMovement unitMovement;
    private Status status;
    private HealthBar healthBar;

    public delegate void EnemyReachedGate(Enemy enemy);
    public delegate void EnemyDied(Enemy enemy);

    public static event EnemyReachedGate OnEnemyReachedGate;
    public static event EnemyDied OnEnemyDied;

    private void Awake() {
        unitInput = new UnitAI(this, pathFinder);
        status = new Status(enemyData, this);
        unitMovement = new UnitMovement(transform.parent.transform, status, unitInput, transform);
        healthBar = transform.parent.GetComponentInChildren<HealthBar>();
    }
        
    private void Update() {
        if (unitMovement != null) {
            unitMovement.Move();
        }
    }

    public void Spawn() {
        unitInput.Initialize();
        status.Initialize();
        unitMovement.Initialize();
        healthBar.Initialize(status, enemyData.BaseHealth);
    }

    public void ReachedDestination() {
        Debug.Log("Unit reached destination");
        if (OnEnemyReachedGate != null) {
            OnEnemyReachedGate.Invoke(this);
        }
        Despawn();
    }

    public void Died() {
        Despawn();
        if (OnEnemyDied != null) {
            OnEnemyDied.Invoke(this);
        }
    }

    private void Despawn() {
        transform.parent.gameObject.SetActive(false);
    }

    public Status GetStatus() {
        return status;
    }

    public void ApplyDamage(Damage.DamageTypeAndAmount[] damages) {
        float damage = Damage.CalculateDamage(status, damages);
        status.TakeDamage(damage);
        healthBar.UpdateHealthBar();
    }

    public Transform GetTransform() {
        return transform;
    }

    public class Factory : PlaceholderFactory<EnemyData.Type, Enemy> { }
}
