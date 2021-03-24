//  https://www.youtube.com/watch?v=XIqtZnqutGg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundLayer;

    [SerializeField]
    private Tilemap decoreLayer;

    [SerializeField]
    private Tilemap structureLayer;

    [SerializeField]
    private List<TileData> tileDatas;

    //  Save from having to add redundant attributes to hundreds of instances of the same tile
    //  https://gameprogrammingpatterns.com/flyweight.html
    private Dictionary<TileBase, TileData> dataFromTiles;

    //  Caching these tiles so it doesnt need to search each time
    private TileBase wallTile;
    private TileBase grassTile;
    private TileBase pathTile;
    private TileBase towerTile;

    [SerializeField]
    private GameObject redTowerPrefab;
    [SerializeField]
    private GameObject blueTowerPrefab;
    [SerializeField]
    private GameObject greenTowerPrefab;

    #region Build Mode variables    
    private BuildMode buildMode = BuildMode.None;
    private TintedTile lastTileHovered;
    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);
    private StructureClass selectedStructureClass;

    //  Keeps track of what type of structure each class of structure is
    //  Example: RedTower class is of type Tower
    Dictionary<StructureClass, StructureType> StructureClassToType;

    //  Stores all the tiles that have been tinted
    private List<TintedTile> tintedtiles;

    //  Stores all the towers that have been built
    private List<InstantiatedTower> instantiatedTowers;

    public delegate void StructureChanged();
    public static event StructureChanged OnStructureChanged;

    [SerializeField]
    private Texture2D buildCursor;
    [SerializeField]
    private Texture2D demolishCursor;
    #endregion

    /// <summary>
    /// TileMap layer
    /// </summary>
    public enum Layer
    {
        GroundLayer,
        DecoreLayer,
        StructureLayer
    }

    /// <summary>
    /// Tiles belonging to the ground layer
    /// </summary>
    public enum GroundTile
    {
        StonePath,
        Grass
    }

    /// <summary>
    /// Tile belonging to the structure layer
    /// </summary>
    public enum StructureType
    {
        Wall,
        TowerBase
    }

    /// <summary>
    /// Specific class of structure
    /// </summary>
    public enum StructureClass
    {
        Wall,
        RedTower,
        BlueTower,
        GreenTower
    }

    public enum BuildMode
    {
        Build,
        Demolish,
        None
    }

    private void Awake()
    {
        //  Link each tile type to their respective shared data attributes
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileDatas)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                //  Cache tiles (I dont like this solution will probably change later)
                if (tile.name == "StonePathRandom")
                {
                    pathTile = tile;
                }
                else if (tile.name == "WallRuleTile")
                {
                    wallTile = tile;
                }
                else if (tile.name == "GrassRandom")
                {
                    grassTile = tile;
                }
                else if (tile.name == "TowerBase")
                {
                    towerTile = tile;
                }

                //  Actually link tile with attributes
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    private void Start()
    {
        Debug.Log("Map manager loaded");
        tintedtiles = new List<TintedTile>();
        instantiatedTowers = new List<InstantiatedTower>();

        StructureClassToType = new Dictionary<MapManager.StructureClass, StructureType>();
        StructureClassToType.Add(StructureClass.RedTower, StructureType.TowerBase);
        StructureClassToType.Add(StructureClass.GreenTower, StructureType.TowerBase);
        StructureClassToType.Add(StructureClass.BlueTower, StructureType.TowerBase);
        StructureClassToType.Add(StructureClass.Wall, StructureType.Wall);

    }

    private void Update()
    {
        //  Build Mode logic
        if (buildMode == BuildMode.Build || buildMode == BuildMode.Demolish)
        {
            //  Exit build/demolish mode if escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitEditMode();
            }
            else
            {
                //  Ignore mouse if its over a UI element, and unhighlight last tile
                if (EventSystem.current.IsPointerOverGameObject() == true)
                {
                    UnhoverBuildTile(lastTileHovered);
                }
                //  Otherwise handle mouse
                else
                {
                    Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    mouseposition.z = 0;

                    //  Handle mouse hovering
                    if (lastTileHovered != null)
                    {
                        if (lastTileHovered.Position != mouseposition)
                        {
                            UnhoverBuildTile(lastTileHovered);
                            lastTileHovered = HoverBuildTile(mouseposition, buildMode);
                        }
                    }
                    else
                    {
                        lastTileHovered = HoverBuildTile(mouseposition, buildMode);
                    }

                    //  Handle left click
                    if (Input.GetMouseButtonDown(0) == true)
                    {
                        if (buildMode == BuildMode.Build)
                        {
                            BuildStructure(mouseposition, selectedStructureClass);
                        }
                        else if (buildMode == BuildMode.Demolish)
                        {
                            DemolishStructure(mouseposition);
                        }
                    }

                }
            }
        }
    }

    /// <summary>
    /// Demolish the structure at a given position
    /// </summary>
    /// <param name="position"></param>
    private void DemolishStructure(Vector3Int position)
    {
        //  Demolish structure 
        if (GetLayer(Layer.StructureLayer).HasTile(position) == true)
        {
            TileBase structureType = GetLayer(Layer.StructureLayer).GetTile(position);
            if (structureType == wallTile)
            {
                RemoveTile(Layer.StructureLayer, position);
            }
            else if (structureType == towerTile)
            {
                //  Search for tower GameObject based on position
                foreach (InstantiatedTower tower in instantiatedTowers)
                {
                    if (tower.Position == position)
                    {
                        // Destroy the tower Gameobject
                        GameObject.Destroy(tower.TowerGameObject);
                        // Stop keeping track of it
                        instantiatedTowers.Remove(tower);
                        break;
                    }
                }
                //  Remove the structure tile representing it
                RemoveTile(Layer.StructureLayer, position);
            }
            else
            {
                throw new Exception("No valid structure is present");
            }

            //  Alert other classes that structure map changed
            if (OnStructureChanged != null)
            {
                OnStructureChanged.Invoke();
            }
        }
    }

    /// <summary>
    /// Build a structure at a given position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="structureClass"></param>
    private void BuildStructure(Vector3Int position, StructureClass structureClass)
    {
        //  Build tower 
        if (GetLayer(Layer.StructureLayer).HasTile(position) == false && GetLayer(Layer.GroundLayer).HasTile(position) == true)
        {
            StructureType structureType = StructureClassToType[structureClass];

            if (structureClass == StructureClass.Wall)
            {
                GetLayer(Layer.StructureLayer).SetTile(position, GetTileBase(structureType));
            }
            else if (StructureClassToType[structureClass] == StructureType.TowerBase)
            {
                GameObject towerPrefab;
                switch (structureClass)
                {
                    case StructureClass.RedTower:
                        towerPrefab = redTowerPrefab;
                        break;
                    case StructureClass.BlueTower:
                        towerPrefab = blueTowerPrefab;
                        break;
                    case StructureClass.GreenTower:
                        towerPrefab = greenTowerPrefab;
                        break;
                    default:
                        throw new Exception("Structure class being built is not implemented");
                }

                //  Instantiate the prefab 
                GameObject towerGO = GameObject.Instantiate(towerPrefab, position + tilemapOffset, new Quaternion(0, 0, 0, 0));
                //  Keep track of tower's position
                InstantiatedTower instantiatedTower = new InstantiatedTower(towerGO, position);
                instantiatedTowers.Add(instantiatedTower);
                //  Add a tile below so the tilemap knows a structure exists
                GetLayer(Layer.StructureLayer).SetTile(position, towerTile);
            }
            else
            {
                throw new Exception("Invalid structure");
            }

            if (OnStructureChanged != null)
            {
                OnStructureChanged.Invoke();
            }
        }
    }

    public void EnterBuildMode(StructureClass structureClass)
    {
        Debug.Log("Entering build mode");
        Vector3Int origin = new Vector3Int(0, 0, 0);
        if (GetLayer(Layer.GroundLayer).HasTile(origin) != true)
        {
            SetTile(origin, GroundTile.Grass);
        }

        lastTileHovered = HoverBuildTile(origin, BuildMode.Build);

        selectedStructureClass = structureClass;
        buildMode = BuildMode.Build;
        Cursor.SetCursor(buildCursor, Vector2.zero, CursorMode.Auto);
    }

    public void ExitEditMode()
    {
        buildMode = BuildMode.None;
        Debug.Log("Exited build mode");
        UnhoverBuildTile(lastTileHovered);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void EnterDemoishMode()
    {
        buildMode = BuildMode.Demolish;
        Cursor.SetCursor(demolishCursor, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// A class representing a tinted tile on the tilemap (because working with tuples is a pain)
    /// </summary>
    private class TintedTile
    {
        public Layer Layer { get; private set; }
        public Vector3Int Position { get; private set; }
        public Color Color { get; private set; }

        public TintedTile PreviouslyTinted { get; private set; }
        public TintedTile(Layer layer, Vector3Int position, Color color, TintedTile previouslyTinted = null)
        {
            Layer = layer;
            Position = position;
            Color = color;
            PreviouslyTinted = previouslyTinted;
        }
    }

    /// <summary>
    /// Stores an instantiated tower and it's position
    /// </summary>
    private class InstantiatedTower
    {
        public GameObject TowerGameObject { get; set; }
        public Vector3Int Position { get; set; }
        public InstantiatedTower(GameObject towerGameObject, Vector3Int position)
        {
            TowerGameObject = towerGameObject;
            Position = position;
        }
    }

    /// <summary>
    /// Select a tile from a specified layer
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position">A world point</param>
    /// <returns></returns>
    public TileBase SelectTile(Layer layer, Vector2 position)
    {
        Tilemap selectLayer = GetLayer(layer);
        Vector3Int gridPosition = selectLayer.WorldToCell(position);
        return selectLayer.GetTile(gridPosition);
    }

    /// <summary>
    /// Select a tile from a specified layer
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position">A point TileMap point</param>
    /// <returns></returns>
    public TileBase SelectTile(Layer layer, Vector3Int position)
    {
        Tilemap selectLayer = GetLayer(layer);
        return selectLayer.GetTile(position);
    }

    /// <summary>
    /// Does a coordinate in specified layer contain a tile?
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool ContainsTileAt(Layer layer, Vector3Int position)
    {
        Tilemap selectLayer = GetLayer(layer);
        if (selectLayer.GetTile(position) != null)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Get TileMap corrisponding to Layers enum
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    private Tilemap GetLayer(Layer layer)
    {
        Tilemap selectLayer = groundLayer;
        switch (layer)
        {
            case Layer.GroundLayer:
                return groundLayer;
            case Layer.DecoreLayer:
                return decoreLayer;
            case Layer.StructureLayer:
                return structureLayer;
            default:
                return null;
        }
    }

    /// <summary>
    /// Get the Ground layer TileBase object represented by an enumerator
    /// </summary>
    /// <param name="groundTile"></param>
    /// <returns></returns>
    private TileBase GetTileBase(GroundTile groundTile)
    {
        switch (groundTile)
        {
            case GroundTile.StonePath:
                return pathTile;
            case GroundTile.Grass:
                return grassTile;
            default:
                throw new Exception("Could not find a ground layer TileBase object associated with this enum");
        }
    }

    /// <summary>
    /// Get the Structure layer TileBase object represented by an enumerator
    /// </summary>
    /// <param name="structureTile"></param>
    /// <returns></returns>
    private TileBase GetTileBase(StructureType structureTile)
    {
        switch (structureTile)
        {
            case StructureType.Wall:
                return wallTile;
            case StructureType.TowerBase:
                return towerTile;
            default:
                throw new Exception("Could not find a structure layer TileBase object associated with this enum");
        }
    }

    /// <summary>
    /// Set a ground tile
    /// </summary>
    /// <param name="position"></param>
    /// <param name="groundTile"></param>
    public void SetTile(Vector3Int position, GroundTile groundTile)
    {
        Tilemap tilemap = GetLayer(Layer.GroundLayer);
        TileBase tileBase = GetTileBase(groundTile);
        tilemap.SetTile(position, tileBase);
    }

    /// <summary>
    /// Set a structure tile
    /// </summary>
    /// <param name="position"></param>
    /// <param name="structureTile"></param>
    public void SetTile(Vector3Int position, StructureType structureTile)
    {
        Tilemap tilemap = GetLayer(Layer.StructureLayer);
        TileBase tileBase = GetTileBase(structureTile);
        tilemap.SetTile(position, tileBase);
    }

    /// <summary>
    /// Removes a tile from a layer at a location
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    public void RemoveTile(Layer layer, Vector3Int position)
    {
        Tilemap tilemap = GetLayer(layer);
        tilemap.SetTile(position, null);
    }

    /// <summary>
    /// Tints a tile 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public void TintTile(Layer layer, Vector3Int position, Color color)
    {
        Tilemap tileMap = GetLayer(layer);

        //  Check if there is a tile at this position
        if (tileMap.HasTile(position) == true)
        {

            //  If this tile is already tinted, remove it from the collection
            foreach (TintedTile tile in tintedtiles)
            {
                if (tile.Layer == layer && tile.Position == position)
                {
                    tintedtiles.Remove(tile);
                    break;
                }
            }

            tileMap.SetTileFlags(position, TileFlags.None);
            tileMap.SetColor(position, color);

            //  Keep track of this tint for later undoing
            tintedtiles.Add(new TintedTile(layer, position, color));
        }
        else
        {
            //Debug.Log("There is no tile at position " + position);
        }
    }

    /// <summary>
    /// Removes the tint from a tile
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="position"></param>
    private void UntintTile(TintedTile tintedTile)
    {
        Tilemap tileMap = GetLayer(tintedTile.Layer);

        //Debug.Log("Removing tint from tile at   " + position);
        tileMap.SetColor(tintedTile.Position, Color.white);
        tintedtiles.Remove(tintedTile);
    }

    /// <summary>
    /// Highlights a tile being hovered over in either red or green depending on the 
    /// build/demolish mode and whether there is a structure present
    /// </summary>
    /// <param name="position"></param>
    /// <returns>An object representing a tinted tile on a specific layer in a given location</returns>
    private TintedTile HoverBuildTile(Vector3Int position, BuildMode buildMode)
    {
        TintedTile hoveredTile = null;
        Tilemap structureLayer = GetLayer(Layer.StructureLayer);
        Tilemap groundLayer = GetLayer(Layer.GroundLayer);
        Color color;

        //  If a structure is present
        if (structureLayer.HasTile(position))
        {
            if (buildMode == BuildMode.Demolish)
            {
                color = Color.green;
            }
            else
            {
                color = Color.red;
            }

            //  If this is a tower highlight the gameobject
            if (structureLayer.GetTile(position) == GetTileBase(StructureType.TowerBase))
            {
                foreach (InstantiatedTower tower in instantiatedTowers)
                {
                    if (tower.Position == position)
                    {
                        SpriteRenderer[] towerSprites = tower.TowerGameObject.GetComponentsInChildren<SpriteRenderer>();
                        towerSprites[0].material.color = color;
                        towerSprites[1].material.color = color;
                        break;
                    }
                }
            }

            structureLayer.SetColor(position, color);
            hoveredTile = new TintedTile(Layer.StructureLayer, position, color);
        }

        //  Otherwise If there is a ground tile
        else if (groundLayer.HasTile(position))
        {
            //  Check if this ground tile is already tinted, so the new tint can be reversed to the old
            //  For example: hovering over a highlighted path tile needs to be reverted back to the previous color after the cursor leaves the tile
            TintedTile previousTile = null;
            foreach (TintedTile tile in tintedtiles)
            {
                if (tile.Layer == Layer.GroundLayer && tile.Position == position)
                {
                    previousTile = tile;
                    tintedtiles.Remove(tile);
                    break;
                }
            }

            groundLayer.SetTileFlags(position, TileFlags.None);
            if (buildMode == BuildMode.Demolish)
            {   
                color = Color.red;
                groundLayer.SetColor(position, Color.red);
            }
            else
            {
                color = Color.green;
                groundLayer.SetColor(position, Color.green);
            }
            hoveredTile = new TintedTile(Layer.GroundLayer, position, color, previousTile);
        }
        else
        {
            //Debug.Log("There is no tile present at " + position);
        }

        //  Keep track of this tinted tile
        if (hoveredTile != null)
        {
            tintedtiles.Add(hoveredTile);
        }

        return hoveredTile;
    }

    /// <summary>
    /// Reverts tile to previous color or removes color if there was none
    /// </summary>
    /// <param name="tile"></param>
    private void UnhoverBuildTile(TintedTile tile)
    {
        tintedtiles.Remove(tile);

        //  If this tile is a tower, unhighlight the gameobject
        if (structureLayer.GetTile(tile.Position) == towerTile)
        {
            foreach (InstantiatedTower tower in instantiatedTowers)
            {
                if (tower.Position == tile.Position)
                {
                    SpriteRenderer[] towerSprites = tower.TowerGameObject.GetComponentsInChildren<SpriteRenderer>();
                    towerSprites[0].material.color = Color.white;
                    towerSprites[1].material.color = Color.white;
                    break;
                }
            }
        }

        if (tile.PreviouslyTinted != null)
        {
            TintTile(tile.Layer, tile.Position, tile.PreviouslyTinted.Color);
        }
        else
        {
            TintTile(tile.Layer, tile.Position, Color.white);
        }
    }

    /// <summary>
    /// Removes the tinting from all tiles
    /// </summary>
    public void ClearAllTints()
    {
        Debug.Log("number of tiles tinted before clearing all tiles:" + tintedtiles.Count);

        while (tintedtiles.Count > 0)
        {
            UntintTile(tintedtiles[0]);
        }

        print("Items in tintedtiles after clearing all tiles: " + tintedtiles.Count);
    }

    /// <summary>
    /// Get the speed attribute of the ground tile occupying a world position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public float GetTileWalkSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = groundLayer.WorldToCell(worldPosition);
        TileBase tile = groundLayer.GetTile(gridPosition);

        if (tile == null)
            return 1f;

        float walkSpeed = dataFromTiles[tile].walkSpeed;
        return walkSpeed;
    }

    /// <summary>
    /// Get tile attributes describing a type of tile
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public TileData GetTileData(TileBase tile)
    {
        return dataFromTiles[tile];
    }

    /// <summary>
    /// Highlights the tiles in an array on the ground layer
    /// </summary>
    /// <param name="path"></param>
    public void HighlightPath(List<Vector3Int> path, Color color)
    {
        foreach (Vector3Int tile in path)
        {
            TintTile(Layer.GroundLayer, tile, color);
        }
    }

    /// <summary>
    /// Intended for quickly clearing map in unity editor
    /// </summary>
    [ContextMenu("Clear all tiles")]
    private void RemoveAllTiles()
    {
        groundLayer.ClearAllTiles();
        structureLayer.ClearAllTiles();
        decoreLayer.ClearAllTiles();
    }
}
