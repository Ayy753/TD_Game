using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    private GameManager gameManager;
    private MapManager mapManager;
    private GUIController guiController;

    //  We will user Vector3Int.down to indicate no tile is being hovered over
    private Vector3Int lastHoveredPosition = Vector3Int.down;
    private List<GameObject> instantiatedTowers;
    private bool lastSelectedStructureWasTower = false;
    public BuildMode CurrentBuildMode { get; private set; } = BuildMode.None;
    private StructureData currentlySelectedStructure;
    private LineRenderer line;

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

        line = GetComponent<LineRenderer>();
        HideRadius();
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
        if (lastSelectedStructureWasTower)
        {
            UnHoverGrid(lastHoveredPosition);
        }
        else
        {
            UnhoverTile(lastHoveredPosition);
        }
        lastHoveredPosition = Vector3Int.down;
    }

    /// <summary>
    /// Attempts to build a structure over a ground tile
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="position"></param>
    private void AttemptBuildStructure(StructureData structure, Vector3Int position)
    {
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
                guiController.SpawnFloatingText(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Can't afford", Color.red);
            }
        }
    }

    /// <summary>
    /// Highlights the tile being hovered over while in build
    /// or demolish mode
    /// </summary>
    /// <param name="position">Mouse cursor position</param>
    private void HoverTile(Vector3Int position)
    {
        if (CurrentBuildMode == BuildMode.Build)
        {
            BuildModeHoverLogic(position);
        }
        else if (CurrentBuildMode == BuildMode.Demolish)
        {
            DemolishModeHoverLogic(position);
        }
        else
        {
            throw new System.Exception("This build mode is not implemented");
        }

        lastHoveredPosition = position;
    }

    /// <summary>
    /// Handles hover logic while in build mode
    /// </summary>
    /// <param name="position"></param>
    private void BuildModeHoverLogic(Vector3Int position)
    {
        MapManager.Layer tileLayer;
        Color tileColor;

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
                if (currentlySelectedStructure.GetType() == typeof(TowerData))
                {
                    BuildTowerHoverLogic(position);
                }
                else
                {
                    mapManager.HighlightTile(tileLayer, position, Color.green);
                }
            }
            else
            {
                mapManager.HighlightTile(tileLayer, position, Color.red);
            }
        }
    }

    /// <summary>
    /// Checks if there is a tower within a 3x3 radius of position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsTowerAdjacent(Vector3Int position)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbour = position + new Vector3Int(x, y, 0);
                if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, neighbour))
                {
                    TileData tileData = mapManager.GetTileData(MapManager.Layer.StructureLayer, neighbour);

                    if (tileData.GetType() == typeof(TowerData))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Highlights 3x3 grid around tower in build mode.
    /// Should only be called if the user is in buildmode, 
    /// has a tower selected, and is hovering over a stable ground tile
    /// </summary>
    /// <param name="position"></param>
    private void BuildTowerHoverLogic(Vector3Int position)
    {
        //  Highlight grid red if another tower is present in it
        if (IsTowerAdjacent(position))
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int neighbor = position + new Vector3Int(x, y, 0);
                    if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, neighbor))
                    {
                        if (mapManager.GetTileData(MapManager.Layer.StructureLayer, neighbor).GetType() == typeof(TowerData))
                        {
                            ChangeTowerTint(neighbor, Color.red);
                        }
                        else
                        {
                            mapManager.HighlightTile(MapManager.Layer.StructureLayer, neighbor, Color.red);
                        }
                    }
                    else
                    {
                        mapManager.HighlightTile(MapManager.Layer.GroundLayer, neighbor, Color.red);
                    }
                }
            }
        }
        //  Otherwise highligh yellow/green
        else
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //  Highlight center green and perimeter yellow
                    if (x == 0 && y == 0)
                    {
                        mapManager.HighlightTile(MapManager.Layer.GroundLayer, position + new Vector3Int(x, y, 0), Color.green);
                    }
                    else
                    {
                        mapManager.HighlightTile(MapManager.Layer.GroundLayer, position + new Vector3Int(x, y, 0), Color.yellow);
                    }
                }
            }
            RenderRadius(position + tilemapOffset, ((TowerData)currentlySelectedStructure).Range);
        }
        lastSelectedStructureWasTower = true;
    }

    /// <summary>
    /// Unhighlights 3x3 grid around tower in build mode
    /// </summary>
    /// <param name="position"></param>
    public void UnHoverGrid(Vector3Int position)
    {
        Debug.Log("unhovering grid");
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                UnhoverTile(new Vector3Int(x, y, 0) + position);
            }
        }
        lastSelectedStructureWasTower = false;
        HideRadius();
    }

    /// <summary>
    /// Handles hover logic while in demolish mode
    /// </summary>
    /// <param name="position"></param>
    private void DemolishModeHoverLogic(Vector3Int position)
    {
        MapManager.Layer tileLayer;
        Color tileColor;

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
            if (gameManager.GameEnded == false)
            {
                if (CurrentBuildMode != BuildMode.None)
                {
                    if (mapManager.ContainsTileAt(MapManager.Layer.GroundLayer, tileCoords) && EventSystem.current.IsPointerOverGameObject() == false)
                    {
                        if (lastSelectedStructureWasTower)
                        {
                            UnHoverGrid(lastHoveredPosition);
                        }
                        else
                        {
                            UnhoverTile(lastHoveredPosition);
                        }

                        HoverTile(tileCoords);
                    }
                    else
                    {
                        PauseHighlighting();
                    }
                }
            }
            else if (lastHoveredPosition != Vector3Int.down)
            {
                UnhoverTile(lastHoveredPosition);
                lastHoveredPosition = Vector3Int.down;
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
        if (CurrentBuildMode != BuildMode.None && gameManager.GameEnded == false &&
            EventSystem.current.IsPointerOverGameObject() == false)
        {
            Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mouseposition.z = 0;

            if (CurrentBuildMode == BuildMode.Build)
            {
                if (currentlySelectedStructure.GetType() == typeof(TowerData))
                {
                    if (IsTowerAdjacent(mouseposition) == true)
                    {
                        guiController.SpawnFloatingTextAtCursor("You cannot build towers next to eachother", Color.red);
                    }
                    else
                    {
                        AttemptBuildStructure(currentlySelectedStructure, mouseposition);
                    }
                }
                else if (mapManager.ContainsTileAt(MapManager.Layer.StructureLayer, mouseposition) == false)
                {
                    AttemptBuildStructure(currentlySelectedStructure, mouseposition);
                }
            }
            else if (CurrentBuildMode == BuildMode.Demolish)
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
    /// Renders a line of a given radius around a point to draw a circle
    /// used to show the radius of things
    /// </summary>
    /// <param name="center">Center of tower</param>
    /// <param name="radius">Attack radius of tower</param>
    public void RenderRadius(Vector3 center, float radius)
    {
        line.enabled = true;
        float x;
        float y;

        //  Drawing a line around the given position
        for (int i = 0; i < line.positionCount; i++)
        {
            x = center.x + radius * Mathf.Sin(Mathf.Deg2Rad * (360 / line.positionCount * i));
            y = center.y + radius * Mathf.Cos(Mathf.Deg2Rad * (360 / line.positionCount * i));

            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    /// <summary>
    /// Hides the line
    /// </summary>
    public void HideRadius()
    {
        line.enabled = false;
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
                    TowerData towerData = tower.GetComponent<Tower>().TowerData;
                    int sellValue = Mathf.RoundToInt(towerData.Cost * 0.66f);
                    gameManager.GainGold(sellValue);
                    guiController.SpawnFloatingTextInCenter("Sold for " + sellValue + " gold", Color.yellow);
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
        CurrentBuildMode = BuildMode.Build;
        Debug.Log("Entered buildmode for structure: " + selectedStructure.name);
    }

    /// <summary>
    /// Enters demolish mode
    /// </summary>
    public void EnterDemolishMode()
    {
        lastHoveredPosition = Vector3Int.zero;
        CurrentBuildMode = BuildMode.Demolish;
        Debug.Log("Entered demolish mode");
    }

    /// <summary>
    /// Exits build/demolish mode
    /// </summary>
    public void ExitBuildMode()
    {
        currentlySelectedStructure = null;
        CurrentBuildMode = BuildMode.None;
        PauseHighlighting();
        Debug.Log("Exited build mode");
    }
}
