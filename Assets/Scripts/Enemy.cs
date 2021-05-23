using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    [Inject]
    IMapManager MapManager;

    [Inject]
    IPathfinder Pathfinder;

    List<Vector3Int> _mainPath;

    // Start is called before the first frame update
    void Start()
    {
        TileData tileData = MapManager.GetTileData(IMapManager.Layer.GroundLayer, Vector3Int.FloorToInt(transform.position));
        if (tileData != null)
        {
            Debug.Log(tileData.ToString());
        }
        else
        {
            Debug.LogError("tiledata is null");
        }

        _mainPath = Pathfinder.GetMainPath();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
