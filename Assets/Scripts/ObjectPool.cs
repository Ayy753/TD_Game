using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //  A generic pool that holds any object that implements IPoolable
    List<IPoolable> PooledObjects;

    void Start()
    {
        PooledObjects = new List<IPoolable>();
    }

    /// <summary>
    /// Takes any object that implements Ipoolable
    /// </summary>
    /// <param name="poolable"></param>
    public GameObject GetObject(GameObject poolable) 
    {
        IPoolable desiredObject = poolable.GetComponent<IPoolable>();

        if (desiredObject != null)
        {
            //  Search for the first item availible with the same prefab as the desired object
            foreach (IPoolable item in PooledObjects)
            {
                if (desiredObject.Prefab == item.Prefab && item.GameObjectRef.activeInHierarchy == false)
                {
                    return item.GameObjectRef;
                }
            }

            GameObject newGameObject = GameObject.Instantiate(desiredObject.Prefab);
            IPoolable newPoolable = newGameObject.GetComponent<IPoolable>();
            PooledObjects.Add(newPoolable);
            return newGameObject;
        }
        else
        {
            throw new System.Exception("The object you're trying to pool does not implement IPoolable");
        }
    }
}
