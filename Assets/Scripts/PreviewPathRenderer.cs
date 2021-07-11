using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PreviewPathRenderer : MonoBehaviour, IPathRenderer {
    [Inject] BuildManager buildManager;
    IPathfinder pathFinder;
    IMapManager mapManager;
    private LineRenderer line;
    private readonly Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);


    private void OnEnable() {
        MouseManager.OnHoveredNewTile += HandleNewTileHovered;
        pathFinder = GameObject.Find("PathFinder").GetComponent<IPathfinder>();
        mapManager = GameObject.Find("MapManager").GetComponent<IMapManager>();
        line = GetComponent<LineRenderer>();
    }

    private void OnDisable() {
        MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
    }

    private void HandleNewTileHovered(Vector3Int tileCoords) {
        if (buildManager.CurrentBuildMode == BuildManager.BuildMode.Build) {
            if (pathFinder.IsOnMainPath(tileCoords)) {
                List<Vector3Int> path = pathFinder.GetBuildPreviewPath(tileCoords);
                RenderPath(path);
            }
        }
        else if (buildManager.CurrentBuildMode == BuildManager.BuildMode.Demolish) {
            if (mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, tileCoords)) {
                List<Vector3Int> path = pathFinder.GetDemolishPreviewPath(tileCoords);
                RenderPath(path);
            }
        }
    }

    public void RenderPath(List<Vector3Int> path) {
        line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++) {
            line.SetPosition(i, path[i] + tilemapOffset);
        }
    }
}
