using System;
using UnityEngine;
using Zenject;

public class Enemy : Unit {
    [Inject] private readonly IPathfinder pathFinder;
    [Inject] private IMessageSystem messageSystem;
    [SerializeField] public EnemyData enemyData;
    private IUnitInput unitInput;
    private IUnitMovement unitMovement;
    private Status status;
    private HealthBar healthBar;

    public delegate void EnemyReachedGate(Enemy enemy);
    public delegate void EnemyDied(Enemy enemy);

    public static event EnemyReachedGate OnEnemyReachedGate;
    public static event EnemyDied OnEnemyDied;
    public override event EventHandler TargetDisabled;

    private void Awake() {
        unitInput = new UnitAI(this, pathFinder);
        //status = new Status(enemyData, this);
        //unitMovement = new UnitMovement(transform.parent.transform, status, unitInput, transform);
        healthBar = transform.parent.GetComponentInChildren<HealthBar>();
    }
        
    private void Update() {
        if (unitMovement != null) {
            unitMovement.Move();
        }
    }

    public void Spawn() {
        unitInput.Initialize();
        //status.Initialize();
        unitMovement.Initialize();
        healthBar.Initialize(status);
    }

    public override void ReachedDestination() {
        Despawn();
        if (OnEnemyReachedGate != null) {
            OnEnemyReachedGate.Invoke(this);
        }
    }

    public override void Died() {
        Despawn();
        if (OnEnemyDied != null) {
            OnEnemyDied.Invoke(this);
        }
    }

    private void Despawn() {
        if (TargetDisabled != null) {
            TargetDisabled.Invoke(this, EventArgs.Empty);
        }
        transform.parent.gameObject.SetActive(false);
    }

    public override Status GetStatus() {
        return status;
    }

    //public override void ApplyDamage(Damage.DamageTypeAndAmount[] damages) {
    //    float damage = Damage.CalculateDamage(status, damages);
    //    status.ModifyDamage(damage);
    //    messageSystem.DisplayMessageAt(transform.position, Math.Round(damage, 1).ToString(), Color.red, 0.33f);
    //    healthBar.UpdateHealthBar();
    //}

    public override Transform GetTransform() {
        return transform;
    }

    public override string GetName() {
        return enemyData.Name;
    }

    public class Factory : PlaceholderFactory<EnemyData.Type, Enemy> { }
}
