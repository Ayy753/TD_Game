using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    [Inject]
    IMapManager MapManager;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
