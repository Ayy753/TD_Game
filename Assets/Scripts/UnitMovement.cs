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

    public UnitMovement(Transform transform, Status unitStatus, IUnitInput unitInput) {
        this.transform = transform;
        this.unitStatus = unitStatus;
        this.unitInput = unitInput;
    }

    public void Initialize() {
        nextTile = unitInput.GetNextTile();
        transform.position = nextTile + TILEMAP_OFFSET;
    }

    public void Move() {
        Vector3 nextTileWithOffset = nextTile + TILEMAP_OFFSET;
        
        if (transform.position != nextTileWithOffset) {
            transform.position = Vector3.MoveTowards(transform.position, nextTileWithOffset, unitStatus.Speed * Time.deltaTime);
        }
        else {
            unitInput.ReachedNextTile();
            nextTile = unitInput.GetNextTile();
        }
    }


}
