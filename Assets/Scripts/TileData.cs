using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Implements a flyweight pattern to share data that is not unique to instances of each tile type
/// https://docs.unity3d.com/Manual/class-ScriptableObject.html
/// 
/// ScriptableObject don't need to be instantiated and are used to store data
/// https://docs.unity3d.com/ScriptReference/ScriptableObject.html
/// </summary>
public abstract class TileData : ScriptableObject
{
    public abstract string Name { get; protected set; }
    public abstract string Description { get; protected set; }
    public abstract Sprite Icon { get; protected set; }
    public abstract MapManager.Layer Layer { get; protected set; }
    public abstract TileBase TileBase { get; protected set; }

    public override string ToString()
    {
        return string.Format("<b>Name</b>: {0}\n<b>Description</b>: {1}", Name, Description);
    }
}
