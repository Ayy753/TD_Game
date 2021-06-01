using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildValidator{
    public bool CanBuildOverTile(Vector3Int position, StructureData structureData);
    public bool CanDemolishStructure(Vector3Int position);
}
