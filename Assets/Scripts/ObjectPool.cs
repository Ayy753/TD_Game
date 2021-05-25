using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class ObjectPool : IFactory<UnityEngine.Object, Enemy>, ITickable, IInitializable
{
    readonly DiContainer _container;
    private GameObject[] enemyPrefabs;

    public ObjectPool(DiContainer container) {
        _container = container;
    }

    public void Initialize() {
        enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        Debug.Log("num prefabs: " + enemyPrefabs.Length);
        for (int i = 0; i < enemyPrefabs.Length; i++) {
            Create(enemyPrefabs[i]);
        }
    }

    public void Tick() {
        //SpawnEnemy();
    }

    public Enemy Create(UnityEngine.Object prefab) {
        return _container.InstantiatePrefabForComponent<Enemy>(prefab);
    }
}
