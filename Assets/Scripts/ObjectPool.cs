using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class ObjectPool : IFactory<UnityEngine.Object, Enemy>, ITickable, IInitializable
{
    IPathfinder pathFinder;
    readonly DiContainer _container;
    private GameObject[] enemyPrefabs;
    private List<Enemy> instantiatedEnemies;
    private GameObject enemyContainer;

    public ObjectPool(DiContainer container, IPathfinder pathFinder) {
        _container = container;
        this.pathFinder = pathFinder;
        instantiatedEnemies = new List<Enemy>(); 
    }

    public void Initialize() {
        enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");

        //  Scene hierarchy container for new enemies
        enemyContainer = new GameObject("Enemy Container");

        //  Preload some enemies
        for (int i = 0; i < enemyPrefabs.Length; i++) {
            for (int j = 0; j < 5; j++) {
                Create(enemyPrefabs[i]);
            }
        }

        //  Disable preloaded enemies
        foreach (Enemy enemy in instantiatedEnemies) {
            enemy.transform.parent.gameObject.SetActive(false);
        }
        
        //  Test
        Create(enemyPrefabs[0]);
    }

    public void Tick() {
        //SpawnEnemy();
    }

    public Enemy Create(UnityEngine.Object prefab) {
        //  Find available enemy of a prefab type
        foreach (Enemy enemy in instantiatedEnemies) {
            if (enemy.enemyData.Prefab == prefab && enemy.gameObject.activeInHierarchy == false) {
                enemy.transform.parent.gameObject.SetActive(true);
                return enemy;
            }
        }

        //  Instantiate new enemy if none are available in pool
        Enemy newEnemy = _container.InstantiatePrefabForComponent<Enemy>(prefab);
        newEnemy.transform.parent.parent = enemyContainer.transform;
        instantiatedEnemies.Add(newEnemy);
        return newEnemy;
    }
}
