using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Implements a flyweight pattern to share data that is not unique to instances of each tile type
/// https://docs.unity3d.com/Manual/class-ScriptableObject.html
/// 
/// ScriptableObject don't need to be instantiated and are used to store data
/// https://docs.unity3d.com/ScriptReference/ScriptableObject.html
/// </summary>
[CreateAssetMenu]
public class TileData : ScriptableObject
{
    //  The tiles this data applies to
    public TileBase[] tiles;
    public float walkSpeed = 1;
    internal float walkspeed;
}
