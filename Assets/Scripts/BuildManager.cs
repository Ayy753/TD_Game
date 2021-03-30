using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public enum BuildMode
    {
        Build,
        Demolish,
        None
    }

    private Vector3Int lastHoveredPosition;
    private List<GameObject> instantiatedTowers;
    private GameObject[] towerPrefabs;
    private StructureData[] structureDatas;
    private BuildMode currentBuildMode;
    private StructureData currentlySelectedStructure;

    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0f);

    //  References to other primary classes
    private GameManager gameManager;
    private MapManager mapManager;

    public void Start()
    {
        gameManager = GameManager.Instance;
        mapManager = gameManager.MapManager;

        instantiatedTowers = new List<GameObject>();
    }

    public void Update()
    {
        HandleHoverLogic();
    }

    public void EnterBuildMode(StructureData selectedStructure)
    {
        currentlySelectedStructure = selectedStructure;
        currentBuildMode = BuildMode.Build;
    }

    public void EnterDemolishMode()
    {
        currentBuildMode = BuildMode.Demolish;
    }

    public void ExitBuildMode()
    {
        currentlySelectedStructure = null;
        currentBuildMode = BuildMode.None;
    }

    private void HandleHoverLogic()
    {
        if (currentBuildMode != BuildMode.None)
        {
            if (lastHoveredPosition == null)
            {
                lastHoveredPosition = new Vector3Int(0, 0, 0);
            }

            Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mouseposition.z = 0;

            if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, mouseposition))
            {
                if (mouseposition != lastHoveredPosition)
                {
                    UnhoverTile(lastHoveredPosition);
                    HoverTile(mouseposition);
                }

                //  build logic
            }
        }
    }

    private void UnhoverTile(Vector3Int position)
    {
        if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, position))
        {
            TileData tile = mapManager.GetTileData(MapManager.Layer.StructureLayer, position);

            if (tile.GetType() == typeof(WallData))
            {
                mapManager.ReverseHighlight(MapManager.Layer.StructureLayer, position);
            }
            else if (tile.GetType() == typeof(TowerData))
            {
                //  Remove sprite tint from tower Game Object
                foreach (GameObject tower in instantiatedTowers)
                {
                    if (tower.transform.position == position + tilemapOffset)
                    {
                        tower.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.white;
                        tower.GetComponentsInChildren<SpriteRenderer>()[1].color = Color.white;
                        break;
                    }
                }
            }
            else
            {
                throw new System.Exception("Structure type not implemented");
            }
        }
    }

    /// <summary>
    /// Highlights the tile being overed over
    /// </summary>
    /// <param name="position">Mouse cursor position</param>
    private void HoverTile(Vector3Int position)
    {
        MapManager.Layer tileLayer;
        Color tileColor;

        if (currentBuildMode == BuildMode.Build)
        {
            if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, position))
            {
                tileLayer = MapManager.Layer.StructureLayer;
                tileColor = Color.red;
            }
            else
            {
                tileLayer = MapManager.Layer.GroundLayer;
                tileColor = Color.green;
            }
        }
        else if (currentBuildMode == BuildMode.Demolish)
        {
            if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, position))
            {
                tileLayer = MapManager.Layer.StructureLayer;
                tileColor = Color.green;
            }
            else
            {
                tileLayer = MapManager.Layer.GroundLayer;
                tileColor = Color.red;
            }
        }
        else
        {
            throw new System.Exception("This build mode is not implemented");
        }

        mapManager.HighlightTile(tileLayer, position, tileColor);
        lastHoveredPosition = position;
    }



}
