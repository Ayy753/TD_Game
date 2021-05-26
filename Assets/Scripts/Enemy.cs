using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, IUnit {
    [Inject] private readonly IPathfinder pathFinder;
    [SerializeField] public EnemyData enemyData;
    private IUnitInput unitInput;
    private Status status;
    private IUnitMovement unitMovement;

    public enum Type {
        Fast,
        Normal,
        Strong
    }

    private void Awake() {
        unitInput = new UnitAI(this, pathFinder);
        status = new Status(enemyData);
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

    private void Despawn() {
        transform.parent.gameObject.SetActive(false);
    }

    public class Factory : PlaceholderFactory<Enemy.Type, Enemy> { }
}
