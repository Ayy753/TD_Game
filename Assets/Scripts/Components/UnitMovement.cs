using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour, IUnitMovement {
    IUnitInput unitInput;
    IMapManager mapManager;
    private static readonly Vector3 TILEMAP_OFFSET = new Vector3(0.5f, 0.5f, 0);
    private Vector3Int nextTile;
    private Status unitStatus;

    private void Awake() {
        unitInput = transform.GetComponent<IUnitInput>();
        mapManager = GameObject.Find("MapManager").GetComponent<IMapManager>();
        unitStatus = transform.GetComponent<Status>();
    }

    private void OnEnable() {
        nextTile = unitInput.GetNextTile();
        transform.parent.position = nextTile + TILEMAP_OFFSET;
    }

    private void Update() {
        Move();
    }

    /// <summary>
    /// Faces next path node
    /// </summary>
    private void FaceNextNode(Vector3Int nextNodePos) {
        //  Rotate unit to face direction of next tile in path
        Vector3 posNoOffset = transform.position - TILEMAP_OFFSET;
        if (nextNodePos.x < posNoOffset.x) {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (nextNodePos.y < posNoOffset.y) {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (nextNodePos.y > posNoOffset.y) {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (nextNodePos.x > posNoOffset.x) {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void Move() {
        Vector3 nextTileWithOffset = nextTile + TILEMAP_OFFSET;

        if (transform.parent.position != nextTileWithOffset) {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, nextTileWithOffset, unitStatus.Speed.Value * Time.deltaTime);
        }
        else {
            unitInput.ReachedNextTile();
            nextTile = unitInput.GetNextTile();
        }

        FaceNextNode(nextTile);
    }
}
