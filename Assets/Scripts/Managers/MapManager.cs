using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

[CreateAssetMenu(fileName = "MapManagerScriptableObject", menuName = "ScriptableObjects/MapManagerScriptableObject")]
public class MapManager : ScriptableObject, IMapManager
{
    private Tilemap groundLayer;
    private Tilemap decoreLayer;
    private Tilemap structureLayer;
    private Tilemap platformLayer;

    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    public void Awake()
    {
        groundLayer = GameObject.Find("GroundLayer").GetComponent<Tilemap>();
        decoreLayer = GameObject.Find("DecorationLayer").GetComponent<Tilemap>();
        structureLayer = GameObject.Find("StructureLayer").GetComponent<Tilemap>();
        platformLayer = GameObject.Find("PlatformLayer").GetComponent<Tilemap>();

        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (TileData tileData in tileDatas)
        {
            Debug.Log("Test");

            //  Link TileBase objects to TileData 
            //  Since towers share the same tower base we need to ensure they dont get added twice
            if (dataFromTiles.ContainsKey(tileData.TileBase) != true)
            {
                Debug.Log(string.Format("Adding: tile name:{0} tilebase:{1}", tileData.name, tileData.TileBase.name));
                dataFromTiles.Add(tileData.TileBase, tileData);
            }
        }
    }

    /// <summary>
    /// Get TileMap corrisponding to Layers enum
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    private Tilemap GetLayer(IMapManager.Layer layer)
    {
        Tilemap selectLayer = groundLayer;
        switch (layer)
        {
            case IMapManager.Layer.GroundLayer:
                return groundLayer;
            case IMapManager.Layer.DecoreLayer:
                return decoreLayer;
            case IMapManager.Layer.PlatformLayer:
                return platformLayer;
            case IMapManager.Layer.StructureLayer:
                return structureLayer;
            default:
                Debug.LogError("Layer not found");
                return null;
        }
    }

    /// <summary>
    /// Gets the TileData class associated with the TileBase on this layer and position
    /// </summary>
    /// <param name="layer">Layer to search</param>
    /// <param name="position">Position of tile</param>
    /// <returns></returns>
    public TileData GetTileData(IMapManager.Layer layer, Vector3Int position)
    {
        //  2D tilemap
        position.z = 0;

        TileBase tile = GetLayer(layer).GetTile(position);
        if (tile != null)
        {
            Debug.Log("found " +tile.ToString());
            return dataFromTiles[tile];
        }
        Debug.Log(string.Format("Did not find tile at layer {0} position {1}", layer, position)) ;
        return null;
    }

    public void SetTile(TileData tileData, Vector3Int position)
    {
        throw new System.NotImplementedException();
    }

}

