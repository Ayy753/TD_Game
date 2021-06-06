using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PathRenderer : MonoBehaviour{
    private LineRenderer line;
    private readonly Vector3 tilemapOffset = new Vector3(0.5f,0.5f,0);

    public void Awake() {
        line = GetComponent<LineRenderer>();
    }

    public void RenderPath(List<Vector3Int> path) {
        line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++) {
            line.SetPosition(i, path[i] + tilemapOffset);
        }
    }
}
