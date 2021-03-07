//  https://www.youtube.com/watch?v=XIqtZnqutGg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;

    private void Start()
    {

    }

    private void Awake()
    {
        //  Link each tile type to their respective shared data attributes
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileDatas)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        BoundsInt cellBounds = map.cellBounds;
        for (int x = cellBounds.xMin; x < cellBounds.xMax; x++)
        {
            for (int y = cellBounds.yMin; y < cellBounds.yMax; y++)
            {
                for (int z = cellBounds.zMin; z < cellBounds.zMax; z++)
                {
                    TileBase tile = map.GetTile(new Vector3Int(x, y, z));
                    if (tile != null)
                    {
                        print(((Tile)tile).colliderType);

                        if (dataFromTiles[tile].passable)
                        {
                            map.SetColliderType(new Vector3Int(x, y, z), Tile.ColliderType.Sprite);
                        }
                        else
                        {
                            map.SetColliderType(new Vector3Int(x, y, z), Tile.ColliderType.None);
                        }
                    }
                }
            }
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            TileBase clickedTile = map.GetTile(gridPosition);
            float walkSpeed = dataFromTiles[clickedTile].walkSpeed;
            print(string.Format("Walk speed on: {0} is {1}", clickedTile, walkSpeed));
        }
    }

    public float GetTileWalkSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tile = map.GetTile(gridPosition);

        if (tile == null)
        {
            return 1f;
        }

        float walkSpeed = dataFromTiles[tile].walkSpeed;

        return walkSpeed;
    }
}
