using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainPathRenderer : MonoBehaviour, IPathRenderer {
    [Inject] private IPathfinder pathfinder;
    private LineRenderer line;
    private readonly Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);

    private void Awake() {
        line = GetComponent<LineRenderer>();
        pathfinder.PathRecalculated += OnPathRecalculated;
    }

    private void OnPathRecalculated(object sender, EventArgs e) {
        RenderPath(pathfinder.GetMainPath());
    }

    public void RenderPath(List<Vector3Int> path) {
        line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++) {
            line.SetPosition(i, path[i] + tilemapOffset);
        }
    }
}