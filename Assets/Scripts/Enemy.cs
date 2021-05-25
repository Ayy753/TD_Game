using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    [Inject] private readonly IPathfinder pathFinder;
    [SerializeField] public EnemyData enemyData;
    private IUnitInput unitInput;
    private Status status;
    private IUnitMovement unitMovement;

    private void Start() {
        unitInput = new UnitAI(pathFinder);
        status = new Status(enemyData);
        unitMovement = new UnitMovement(transform.parent.transform, status, unitInput);
    }

    private void Update() {
        if (unitMovement != null) {
            unitMovement.Move();
        }
    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, Enemy> { }
}
