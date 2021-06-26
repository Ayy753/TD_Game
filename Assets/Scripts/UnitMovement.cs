using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : IUnitMovement
{
    IUnitInput unitInput;
    private static readonly Vector3 TILEMAP_OFFSET = new Vector3(0.5f, 0.5f, 0);
    private readonly Transform transform;
    private Vector3Int nextTile;
    private Status unitStatus;
    private Transform body;

    public UnitMovement(Transform transform, Status unitStatus, IUnitInput unitInput, Transform body) {
        this.transform = transform;
        this.unitStatus = unitStatus;
        this.unitInput = unitInput;
        this.body = body;
    }

    public void Initialize() {
        nextTile = unitInput.GetNextTile();
        transform.position = nextTile + TILEMAP_OFFSET;
    }

    /// <summary>
    /// Faces next path node
    /// </summary>
    private void FaceNextNode(Vector3Int nextNodePos) {
        //  Rotate unit to face direction of next tile in path
        Vector3 posNoOffset = body.position - TILEMAP_OFFSET;
        if (nextNodePos.x < posNoOffset.x) {
            body.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (nextNodePos.y < posNoOffset.y) {
            body.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (nextNodePos.y > posNoOffset.y) {
            body.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (nextNodePos.x > posNoOffset.x) {
            body.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void Move() {
        Vector3 nextTileWithOffset = nextTile + TILEMAP_OFFSET;
        
        if (transform.position != nextTileWithOffset) {
            transform.position = Vector3.MoveTowards(transform.position, nextTileWithOffset, unitStatus.GetStat(Status.StatType.Speed) * Time.deltaTime);
        }
        else {
            unitInput.ReachedNextTile();
            nextTile = unitInput.GetNextTile();
        }

        FaceNextNode(nextTile);
    }
}
