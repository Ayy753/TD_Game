using UnityEngine;

public interface IPoolable
{
    /// <summary>
    /// Get the object's prefab which is to be instantiated
    /// Interfaces do not allow private setters but the prefab is not meant to be changed
    /// therefore the setter is not included
    /// </summary>
    public GameObject Prefab { get; }

    /// <summary>
    /// Reference to the GameObject of the object
    /// </summary>
    public GameObject GameObjectRef { get; }

    /// <summary>
    /// Set the position and parent of the object that is instantiated
    /// </summary>
    /// <param name="position"></param>
    /// <param name="parent"></param>
    public void Spawn(Vector3 position, Transform parent);

}
