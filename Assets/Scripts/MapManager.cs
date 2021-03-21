//  https://www.youtube.com/watch?v=XIqtZnqutGg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private GameObject towerPrefab;

    //  Build mode stuff
    private BuildMode buildMode = BuildMode.None;
    private TintedTile lastTileHovered;
    private Vector3 tilemapOffset = new Vector3(0.5f, 0.5f, 0);

    //  Stores all the tiles that have been tinted
    private List<TintedTile> tintedtiles;

    //  Stores all the towers that have been built
    private List<InstantiatedTower> instantiatedTowers;

    public delegate void StructureChanged();
    public static event StructureChanged OnStructureChanged;

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
    public enum StructureTile
    {
        Wall,
        TowerBase
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
    }

    private void Update()
    {
        if (buildMode == BuildMode.Build || buildMode == BuildMode.Demolish)
        {
            Vector3Int mouseposition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mouseposition.z = 0;

            //  Handle hovering
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

            //  Handle click
            if (Input.GetMouseButtonDown(0) == true)
            {
                if (buildMode == BuildMode.Build)
                {
                    //  Build tower 
                    if (GetLayer(Layer.StructureLayer).HasTile(mouseposition) == false && GetLayer(Layer.GroundLayer).HasTile(mouseposition) == true)
                    {
                        //  Instantiate the prefab 
                        GameObject towerGO = GameObject.Instantiate(towerPrefab, mouseposition + tilemapOffset, new Quaternion(0, 0, 0, 0));
                        //  Keep track of tower's position
                        InstantiatedTower instantiatedTower = new InstantiatedTower(towerGO, mouseposition);
                        instantiatedTowers.Add(instantiatedTower);
                        //  Add a tile below so the tilemap knows a structure exists
                        GetLayer(Layer.StructureLayer).SetTile(mouseposition, towerTile);
                        if (OnStructureChanged != null)
                        {
                            OnStructureChanged.Invoke();
                        }
                    }
                }
                else if (buildMode == BuildMode.Demolish)
                {
                    //  Demolish tower 
                    if (GetLayer(Layer.StructureLayer).HasTile(mouseposition) == true)
                    {
                        //  Search for tower GameObject based on position
                        foreach (InstantiatedTower tower in instantiatedTowers)
                        {
                            if (tower.Position == mouseposition)
                            {
                                //  Destroy the tower Gameobject, remove the tile representing it, and stop keeping track of it
                                GameObject.Destroy(tower.TowerGameObject);
                                RemoveTile(Layer.StructureLayer, mouseposition);
                                instantiatedTowers.Remove(tower);
                                if (OnStructureChanged != null)
                                {
                                    OnStructureChanged.Invoke();
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("not in build mode");
        }
    }

    public void EnterBuildMode()
    {
        Debug.Log("Entering build mode");
        Vector3Int origin = new Vector3Int(0, 0, 0);
        if (GetLayer(Layer.GroundLayer).HasTile(origin) != true)
        {
            SetTile(origin, GroundTile.Grass);
        }

        lastTileHovered = HoverBuildTile(origin, BuildMode.Build);

        //  I should start writing unit tests
        if (lastTileHovered == null)
        {
            throw new Exception("Hovered tile is null");
        }

        buildMode = BuildMode.Build;
    }

    public void ExitEditMode()
    {
        buildMode = BuildMode.None;
        Debug.Log("Exited build mode");
    }

    public void EnterDemoishMode()
    {
        buildMode = BuildMode.Demolish;
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
    /// Get the TileBase object occupying this position on the ground layer
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
                return null;
        }
    }

    /// <summary>
    /// Get the TileBase object occupying this position on the structure layer
    /// </summary>
    /// <param name="structureTile"></param>
    /// <returns></returns>
    private TileBase GetTileBase(StructureTile structureTile)
    {
        switch (structureTile)
        {
            case StructureTile.Wall:
                return wallTile;
            default:
                return null;
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
    public void SetTile(Vector3Int position, StructureTile structureTile)
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
            Debug.Log("There is no tile at position " + position);
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
            structureLayer.SetTileFlags(position, TileFlags.None);
            if (buildMode == BuildMode.Demolish)
            {
                color = Color.green;
                structureLayer.SetColor(position, color);
            }
            else
            {
                color = Color.red;
                structureLayer.SetColor(position, color);
            }
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


}
