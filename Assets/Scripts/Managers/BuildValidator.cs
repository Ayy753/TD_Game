using UnityEngine;
using Zenject;

public class BuildValidator : IBuildValidator{
    [Inject] IMapManager mapManager;

    public BuildValidator(IMapManager mapManager) { 
        this.mapManager = mapManager;
    }
    public bool CanBuildOverTile(Vector3Int position, StructureData selectedStructure) {
        if (mapManager.ContainsTileAt(IMapManager.Layer.GroundLayer, position) == false
            || mapManager.ContainsTileAt(IMapManager.Layer.StructureLayer, position) == true
            || mapManager.IsGroundSolid(position) == false) {
            return false;
        }

        if (selectedStructure.GetType() == typeof(TowerData)
            && IsTowerAtOrAdjacent(position)) {
            return false;
        }

        return true;
    }

    public bool CanDemolishStructure(Vector3Int position) {
        StructureData structureData = (StructureData)mapManager.GetTileData(IMapManager.Layer.StructureLayer, position);
        if (structureData != null && structureData.Demolishable == true) {
            return true;
        }
        return false;
    }

    private bool IsTowerAtOrAdjacent(Vector3Int position) {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                Vector3Int neighbour = position + new Vector3Int(x, y, 0);
                StructureData structureData = (StructureData)mapManager.GetTileData(IMapManager.Layer.StructureLayer, neighbour);
                if (structureData != null && structureData.GetType() == typeof(TowerData)) {
                    return true;
                }
            }
        }
        return false;
    }
}
