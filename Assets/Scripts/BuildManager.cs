using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class BuildManager : MonoBehaviour
{
    private GameManager gameManager;
    private MapManager mapManager;
    private GUIController guiController;

    //  We will user Vector3Int.down to indicate no tile is being hovered over
    private Vector3Int lastHoveredPosition = Vector3Int.down;
    private List<GameObject> instantiatedTowers;
    private BuildMode currentBuildMode = BuildMode.None;
    private StructureData currentlySelectedStructure;
    
    [SerializeField]
    private GameObject radiusIndicator;

    //  An offset for GameObjects to align properly with the tilemap
    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0f);

    public enum BuildMode
    {
        Build,
        Demolish,
        None
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        mapManager = gameManager.MapManager;
        guiController = gameManager.GUIController;
    }

    private void OnEnable()
    {
        instantiatedTowers = new List<GameObject>();

        MouseManager.OnHoveredNewTile += HandleNewTileHovered;
        MouseManager.OnMouseUp += HandleMouseUp;
    }

    private void OnDisable()
    {
        MouseManager.OnHoveredNewTile -= HandleNewTileHovered;
        MouseManager.OnMouseUp -= HandleMouseUp;
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
    /// Builds a structure over a ground tile
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    private void BuildStructure(StructureData structure, Vector3Int position)
    {
        //  Ensure there is a ground to be built upon
        if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, position))
        {
            if (gameManager.CanAfford(structure.Cost))
            {
                if (mapManager.IsGroundSolid(position))
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

                    gameManager.SpendGold(structure.Cost);
                    guiController.SpawnFloatingText(Camera.main.ScreenToWorldPoint(Input.mousePosition), string.Format("Spent {0}g", structure.Cost), Color.yellow);
                }
                else
                {
                    guiController.SpawnFloatingText(Camera.main.ScreenToWorldPoint(Input.mousePosition), "It is too unstable to build here", Color.red);
                }
            }
            else
            {
                guiController.SpawnFloatingText(Camera.main.ScreenToWorldPoint(Input.mousePosition) , "Can't afford", Color.red);
            }
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
            //  Is there a structure here?
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
            //  There is no structure here
            else
            {
                tileLayer = MapManager.Layer.GroundLayer;
                //  Is tile capable of supporting a structure?
                if (mapManager.IsGroundSolid(position))
                {
                    tileColor = Color.green;

                    //  If the selected structure is a tower, display the targetting radius
                    if (currentlySelectedStructure.GetType() == typeof(TowerData))
                    {
                        TowerData towerData = (TowerData)currentlySelectedStructure;
                        radiusIndicator.SetActive(true);
                        radiusIndicator.transform.localScale = new Vector3(towerData.Range * 2, towerData.Range * 2, 0);
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        mousePos = Vector3Int.FloorToInt(mousePos);
                        mousePos.z = 1;
                        radiusIndicator.transform.position = mousePos + tilemapOffset;
                    }
                }
                else
                {
                    tileColor = Color.red;
                }
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
        //  Temp until I fix script load order
        if (mapManager != null)
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

        radiusIndicator.SetActive(false);
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
        GameObject tower = GameObject.Instantiate(towerData.TowerPrefab, position + tilemapOffset, new Quaternion(0, 0, 0, 0));
        instantiatedTowers.Add(tower);
    }

    /// <summary>
    /// Handles logic when a new tile is hovered
    /// </summary>
    /// <param name="tileCoords"></param>
    private void HandleNewTileHovered(Vector3Int tileCoords)
    {
        //  Temp until I fix script load order
        if (mapManager != null)
        {
            if (currentBuildMode != BuildMode.None )
            {
                if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, tileCoords) && EventSystem.current.IsPointerOverGameObject() == false)
                {
                    UnhoverTile(lastHoveredPosition);
                    HoverTile(tileCoords);
                }
                else
                {
                    PauseHighlighting();
                }
            }
        }
    }

    /// <summary>
    /// Responds to a complete mouse click
    /// If in build mode, it will attempt to build a structure
    /// 
    /// </summary>
    private void HandleMouseUp()
    {
        Debug.Log("Mouse button up");
        if (currentBuildMode != BuildMode.None && EventSystem.current.IsPointerOverGameObject() == false)
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
    /// Demolishes a structure
    /// </summary>
    /// <param name="position"></param>
    public void DemolishStructure(Vector3Int position)
    {
        TileData structure = mapManager.GetTileData(MapManager.Layer.StructureLayer, position);
        if (structure.GetType() == typeof(TowerData))
        {
            //  Find and remove tower at this position
            foreach (GameObject tower in instantiatedTowers)
            {
                if (tower.transform.position == position + tilemapOffset)
                {
                    int value = Mathf.RoundToInt(((TowerData)structure).Cost * 0.66f);
                    gameManager.GainGold(value);
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
    /// Enters build mode
    /// </summary>
    /// <param name="selectedStructure"></param>
    public void EnterBuildMode(StructureData selectedStructure)
    {
        currentlySelectedStructure = selectedStructure;
        lastHoveredPosition = Vector3Int.zero;
        currentBuildMode = BuildMode.Build;
        Debug.Log("Entered buildmode for structure: " + selectedStructure.name);
    }

    /// <summary>
    /// Enters demolish mode
    /// </summary>
    public void EnterDemolishMode()
    {
        lastHoveredPosition = Vector3Int.zero;
        currentBuildMode = BuildMode.Demolish;
        Debug.Log("Entered demolish mode");
    }

    /// <summary>
    /// Exits build/demolish mode
    /// </summary>
    public void ExitBuildMode()
    {
        currentlySelectedStructure = null;
        currentBuildMode = BuildMode.None;
        PauseHighlighting();
        Debug.Log("Exited build mode");
    }
}
