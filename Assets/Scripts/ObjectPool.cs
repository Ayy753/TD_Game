using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class ObjectPool : IInitializable
{
    private readonly DiContainer _container;
    private readonly GameObject[] enemyPrefabs;
    private List<Enemy> instantiatedEnemies;
    private GameObject enemyContainer;
    private Dictionary<EnemyData.Type, GameObject> enemyTypeToPrefab;

    public ObjectPool(DiContainer container) {
        _container = container;
        enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
    }

    public void Initialize() {
        Debug.Log("Initializing ObjectPool");

        instantiatedEnemies = new List<Enemy>();
        enemyTypeToPrefab = new Dictionary<EnemyData.Type, GameObject>();

        for (int i = 0; i < enemyPrefabs.Length; i++) {
            GameObject prefab = enemyPrefabs[i];
            switch (prefab.name) {
                case "FastEnemy":
                    enemyTypeToPrefab.Add(EnemyData.Type.Fast, prefab);
                    break;
                case "NormalEnemy":
                    enemyTypeToPrefab.Add(EnemyData.Type.Normal, prefab);
                    break;
                case "StrongEnemy":
                    enemyTypeToPrefab.Add(EnemyData.Type.Strong, prefab);
                    break;
                default:
                    throw new System.Exception(string.Format("Enemy prefab name \"{0}\" does not match any Enemy.Type values ", prefab.name));
            }
        }

        //  Scene hierarchy container for new enemies
        enemyContainer = new GameObject("Enemy Container");

        //  Preload some enemies
        foreach (EnemyData.Type type in enemyTypeToPrefab.Keys) {
            for (int j = 0; j < 5; j++) {
                Create(type);
            }
        }

        //  Disable preloaded enemies
        foreach (Enemy enemy in instantiatedEnemies) {
            enemy.transform.parent.gameObject.SetActive(false);
        }
    }

    public Enemy Create(EnemyData.Type type) {
        //  Find available enemy of a prefab type
        foreach (Enemy enemy in instantiatedEnemies) {
            if (enemy.enemyData.MyType == type && enemy.gameObject.activeInHierarchy == false) {
                enemy.transform.parent.gameObject.SetActive(true);
                return enemy;
            }
        }

        //  Instantiate new enemy if none are available in pool
        GameObject prefab = enemyTypeToPrefab[type];
        Enemy newEnemy = _container.InstantiatePrefabForComponent<Enemy>(prefab);

        newEnemy.transform.parent.parent = enemyContainer.transform;
        instantiatedEnemies.Add(newEnemy);
        return newEnemy;
    }
}
