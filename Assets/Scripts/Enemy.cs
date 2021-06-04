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


    private void Awake() {
        unitInput = new UnitAI(this, pathFinder);
        status = new Status(enemyData, this);
        unitMovement = new UnitMovement(transform.parent.transform, status, unitInput);
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
    }

    public void ReachedDestination() {
        Debug.Log("Unit reached destination");
        //  TODO: fire event or something
        Despawn();
    }

    public void Died() {
        Despawn();
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
    }

    public class Factory : PlaceholderFactory<EnemyData.Type, Enemy> { }
}
