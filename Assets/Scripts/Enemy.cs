using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    [Inject]
    IPathfinder pathfinder;

    private IUnitInput unitInput;
    private Status status;
    private IUnitMovement unitMovement;

    [SerializeField] CharacterData characterData;

    private IEnumerator Start() {
        yield return new WaitForSeconds(2f);
        unitInput = new UnitAI(pathfinder);
        status = new Status(characterData);
        unitMovement = new UnitMovement(transform.parent.transform, status, unitInput);
    }

    private void Update() {
        if (unitMovement != null) {
            unitMovement.Move();
        }
    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, Enemy> { }


}
