using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public enum BuildMode
    {
        Build,
        Demolish,
        None
    }

    //  We will user Vector3Int.down to indicate no tile is being hovered over
    private Vector3Int lastHoveredPosition = Vector3Int.down;
    private List<GameObject> instantiatedTowers;
    private BuildMode currentBuildMode = BuildMode.None;
    private StructureData currentlySelectedStructure;

    //  An offset for GameObjects to align properly with the tilemap
    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0f);

    //  References to other primary classes
    private GameManager gameManager;
    private MapManager mapManager;

    public void Start()
    {
        gameManager = GameManager.Instance;
        mapManager = gameManager.MapManager;

        instantiatedTowers = new List<GameObject>();

        //  Adding a ground tile to origin if there is nothing there
        //  Because the last hovered tile gets initialized at origin when entering build/demolish mode
        if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, Vector3Int.zero) == false)
        {
            mapManager.SetTile(lastHoveredPosition, MapManager.GroundTile.Grass);
            Debug.Log("Added grass tile to origin");
        }
    }

    public void Update()
    {
        //  Prevent clicking through GUI elements
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            HandleHoverLogic();
            HandleClickLogic();
        }
        else
        {
            PauseHighlighting();
        }
    }

    public void EnterBuildMode(StructureData selectedStructure)
    {
        currentlySelectedStructure = selectedStructure;
        currentBuildMode = BuildMode.Build;
        Debug.Log("Entered buildmode for structure: " + selectedStructure.name);
    }

    public void EnterDemolishMode()
    {
        currentBuildMode = BuildMode.Demolish;
        Debug.Log("Entered demolish mode");
    }

    public void ExitBuildMode()
    {
        currentlySelectedStructure = null;
        currentBuildMode = BuildMode.None;
        PauseHighlighting();
        Debug.Log("Exited build mode");
    }

    /// <summary>
    /// Handles tile highlighting under cursor while in build/demolish mode
    /// </summary>
    private void HandleHoverLogic()
    {
        if (currentBuildMode != BuildMode.None)
        {
            Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mouseposition.z = 0;

            if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, mouseposition))
            {
                if (mouseposition != lastHoveredPosition)
                {
                    if (IsATileCurrentlyHovered())
                    {
                        UnhoverTile(lastHoveredPosition);
                    }
                    HoverTile(mouseposition);
                }
            }
            else
            {
                PauseHighlighting();
            }
        }
    }

    /// <summary>
    /// Used when cursor hovers over GUI elements or empty space
    /// or when user exits build/demolish mode
    /// </summary>
    private void PauseHighlighting()
    {
        UnhoverTile(lastHoveredPosition);
        lastHoveredPosition = Vector3Int.down;
    }

    /// <summary>
    /// Handles build mode click logic
    /// </summary>
    private void HandleClickLogic()
    {
        if (currentBuildMode != BuildMode.None && Input.GetMouseButtonDown(0))
        {
            Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mouseposition.z = 0;

            if (currentBuildMode == BuildMode.Build)
            {
                if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, mouseposition) == false)
                {
                    BuildStructure(currentlySelectedStructure, mouseposition);
                }
            }
            else if (currentBuildMode == BuildMode.Demolish)
            {
                if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, mouseposition))
                {
                    DemolishStructure(mouseposition);
                }
            }
            else
            {
                throw new System.Exception("This build mode is not implemented");
            }
        }
    }

    /// <summary>
    /// Builds a structure over a ground tile
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    private void BuildStructure(StructureData structure, Vector3Int position)
    {
        //  Ensure there is a ground to be built upon
        if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, position))
        {
            if (structure.GetType() == typeof(TowerData))
            {
                InstantiateTower((TowerData)structure, position);
                mapManager.SetTile(position, structure);
            }
            else if (structure.GetType() == typeof(WallData))
            {
                mapManager.SetTile(position, structure);
            }
            else
            {
                throw new System.Exception("Structure type " + structure.GetType() + " not implemented");
            }
        }
    }

    /// <summary>
    /// Demolishes a structure
    /// </summary>
    /// <param name="position"></param>
    private void DemolishStructure(Vector3Int position)
    {
        TileData structure = mapManager.GetTileData(MapManager.Layer.StructureLayer, position);
        if (structure.GetType() == typeof(TowerData))
        {
            //  Find and remove tower at this position
            foreach (GameObject tower in instantiatedTowers)
            {
                if (tower.transform.position == position + tilemapOffset)
                {
                    instantiatedTowers.Remove(tower);
                    GameObject.Destroy(tower);
                    mapManager.RemoveTile(MapManager.Layer.StructureLayer, position);
                    break;
                }
            }
        }
        else if (structure.GetType() == typeof(WallData))
        {
            mapManager.RemoveTile(MapManager.Layer.StructureLayer, position);
        }
        else
        {
            throw new System.Exception("Stucture type " + structure.GetType() + " not implemented");
        }
    }

    /// <summary>
    /// Highlights the tile being hovered over
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
                StructureData tile = (StructureData)mapManager.GetTileData(MapManager.Layer.StructureLayer, position);

                if (tile.GetType() == typeof(TowerData))
                {
                    ChangeTowerTint(position, Color.red);
                }
                else
                {
                    tileLayer = MapManager.Layer.StructureLayer;
                    tileColor = Color.red;
                    mapManager.HighlightTile(tileLayer, position, tileColor);
                }
            }
            else
            {
                tileLayer = MapManager.Layer.GroundLayer;
                tileColor = Color.green;
                mapManager.HighlightTile(tileLayer, position, tileColor);
            }
        }
        else if (currentBuildMode == BuildMode.Demolish)
        {
            if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, position))
            {
                StructureData tile = (StructureData)mapManager.GetTileData(MapManager.Layer.StructureLayer, position);

                if (tile.GetType() == typeof(TowerData))
                {
                    ChangeTowerTint(position, Color.green);
                }
                else
                {
                    tileLayer = MapManager.Layer.StructureLayer;
                    tileColor = Color.green;
                    mapManager.HighlightTile(tileLayer, position, tileColor);
                }
            }
            else
            {
                tileLayer = MapManager.Layer.GroundLayer;
                tileColor = Color.red;
                mapManager.HighlightTile(tileLayer, position, tileColor);
            }
        }
        else
        {
            throw new System.Exception("This build mode is not implemented");
        }

        lastHoveredPosition = position;
    }

    /// <summary>
    /// Unhighlights a tile
    /// </summary>
    /// <param name="position"></param>
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
                //  Add previous tower color property to tower script?
                //  So far nothing else can highlight structures so we would never need to revert it
                ChangeTowerTint(position, Color.white);
            }
            else
            {
                throw new System.Exception("Structure type not implemented");
            }
        }
        else
        {
            mapManager.ReverseHighlight(MapManager.Layer.GroundLayer, position);
        }
    }

    /// <summary>
    /// "Highlights" a tower game object at a tilemap position
    /// </summary>
    /// <param name="position">Tilemap position (without offset)</param>
    /// <param name="color">Color to change tint to</param>
    private void ChangeTowerTint(Vector3Int position, Color color)
    {
        foreach (GameObject tower in instantiatedTowers)
        {
            if (tower.transform.position == position + tilemapOffset)
            {
                tower.GetComponentsInChildren<SpriteRenderer>()[0].color = color;
                tower.GetComponentsInChildren<SpriteRenderer>()[1].color = color;
                break;
            }
        }
    }

    /// <summary>
    /// Instantiates and initializes a tower game object and adds it to list
    /// </summary>
    /// <param name="towerData"></param>
    /// <param name="position"></param>
    private void InstantiateTower(TowerData towerData, Vector3Int position)
    {
        GameObject tower = GameObject.Instantiate(towerData.Prefab, position + tilemapOffset, new Quaternion(0, 0, 0, 0));
        tower.GetComponent<Tower>().SetTowerData(towerData);
        instantiatedTowers.Add(tower);
    }

    /// <summary>
    /// Indicates whether a tile is being hovered or not
    /// </summary>
    /// <returns></returns>
    private bool IsATileCurrentlyHovered()
    {
        if (lastHoveredPosition != Vector3Int.down)
        {
            return true;
        }
        return false;
    }
}
