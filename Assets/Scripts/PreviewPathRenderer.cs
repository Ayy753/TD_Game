using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PreviewPathRenderer : MonoBehaviour, IPathRenderer {
    [Inject] BuildManager buildManager;
    [Inject] IBuildValidator buildValidator;

    IPathfinder pathFinder;
    IMapManager mapManager;
    private LineRenderer line;
    private readonly Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);

    private void Awake() {
        pathFinder = GameObject.Find("PathFinder").GetComponent<IPathfinder>();
        mapManager = GameObject.Find("MapManager").GetComponent<IMapManager>();
        line = GetComponent<LineRenderer>();
    }

    private void OnEnable() {
        MouseManager.OnHoveredNewTile += HandleNewTileHovered;
        MouseManager.OnLeftMouseUp += ClearPath;
        MouseManager.OnRightMouseUp += ClearPath;
    }

    private void OnDisable() {
        MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
        MouseManager.OnLeftMouseUp -= ClearPath;
        MouseManager.OnRightMouseUp += ClearPath;
    }

    private void HandleNewTileHovered(Vector3Int tileCoords) {
        if (buildManager.CurrentBuildMode == BuildManager.BuildMode.Build) {
            if (pathFinder.IsOnMainPath(tileCoords) && buildValidator.IsPositionBuildable(tileCoords)) {
                List<Vector3Int> path = pathFinder.GetBuildPreviewPath(tileCoords);
                RenderPath(path);
            }
            else {
                ClearPath();
            }
        }
        else if (buildManager.CurrentBuildMode == BuildManager.BuildMode.Demolish) {
            if (buildValidator.IsStructurePresentAndDemolishable(tileCoords)) {
                List<Vector3Int> path = pathFinder.GetDemolishPreviewPath(tileCoords);
                RenderPath(path);
            }
            else {
                ClearPath();
            }
        }
    }

    public void RenderPath(List<Vector3Int> path) {
        if (path != null) {
            line.positionCount = path.Count;

            for (int i = 0; i < path.Count; i++) {
                line.SetPosition(i, path[i] + tilemapOffset);
            }
        }
    }

    public void ClearPath() {
        if (line != null) {
            line.positionCount = 0;
        }
    }
}
