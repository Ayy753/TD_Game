using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    [SerializeField]
    private Tilemap structureLayer;

    [SerializeField]
    GameObject entrance;

    [SerializeField]
    GameObject exit;

    List<Vector3Int> openList;
    List<Vector3Int> closedList;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Path Finder loaded");

        openList = new List<Vector3Int>();
        closedList = new List<Vector3Int>();

        if (GameManager.Instance.MapManager == null)
        {
            print("mapmanager is null");
        }
        else if (entrance == null)
        {
            print("exit is null");
        }
        else
        {
            print(entrance.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
