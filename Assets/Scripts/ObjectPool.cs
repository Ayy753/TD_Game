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

    private readonly GameObject[] projectilePrefabs;
    private List<Projectile> instantiatedProjectiles;
    private GameObject projectileContainer;
    private Dictionary<ProjectileData.ProjectileType, GameObject> projectileTypeToPrefab;

    public ObjectPool(DiContainer container) {
        _container = container;
        enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        projectilePrefabs = Resources.LoadAll<GameObject>("Prefabs/Projectiles");
    }

    public void Initialize() {
        Debug.Log("Initializing ObjectPool");

        instantiatedEnemies = new List<Enemy>();
        enemyTypeToPrefab = new Dictionary<EnemyData.Type, GameObject>();

        instantiatedProjectiles = new List<Projectile>();
        projectileTypeToPrefab = new Dictionary<ProjectileData.ProjectileType, GameObject>();

        //  Link enemy types to prefabs
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

        //  Link projectile types to prefabs
        for (int i = 0; i < projectilePrefabs.Length; i++) {
            GameObject prefab = projectilePrefabs[i];
            switch (prefab.name) {
                case "NormalProjectile":
                    projectileTypeToPrefab.Add(ProjectileData.ProjectileType.Normal, prefab);
                    break;
                case "SplashProjectile":
                    projectileTypeToPrefab.Add(ProjectileData.ProjectileType.Splash, prefab);
                    break;
                default:
                    throw new System.Exception(string.Format("Projectile prefab name \"{0}\" does not match any ProjectileData.ProjectileType values", prefab.name));
            }
        }

        //  Scene hierarchy container for new enemies
        enemyContainer = new GameObject("Enemy Container");
        projectileContainer = new GameObject("Projectile Container");

        //  Preload some enemies
        foreach (EnemyData.Type type in enemyTypeToPrefab.Keys) {
            for (int j = 0; j < 5; j++) {
                CreateEnemy(type);
            }
        }

        //  Preload some projectiles
        foreach (ProjectileData.ProjectileType type in projectileTypeToPrefab.Keys) {
            for (int j = 0; j < 5; j++) {
                CreateProjectile(type);
            }
        }

        //  Disable preloaded enemies
        foreach (Enemy enemy in instantiatedEnemies) {
            enemy.transform.parent.gameObject.SetActive(false);
        }

        //  Disable preloaded projectiles
        foreach (Projectile projectile in instantiatedProjectiles) {
            projectile.transform.gameObject.SetActive(false);
        }
    }

    public Enemy CreateEnemy(EnemyData.Type type) {
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

    public Projectile CreateProjectile(ProjectileData.ProjectileType type) {
        //  find available projectile of prefab type
        foreach (Projectile projectile in instantiatedProjectiles) {
            if (projectile.ProjectileData.type == type && projectile.gameObject.activeInHierarchy == false) {
                projectile.gameObject.SetActive(true);
                return projectile;
            }
        }

        //  instanteiate new projectile if none are available in pool
        GameObject prefab = projectileTypeToPrefab[type];
        Projectile newProjectile = _container.InstantiatePrefabForComponent<Projectile>(prefab);

        newProjectile.transform.parent = projectileContainer.transform;
        instantiatedProjectiles.Add(newProjectile);
        return newProjectile;
    }

    //  Don't know if its worth pooling towers but other objects will create/destroy them via object pool
    public Tower CreateTower(TowerData towerData) {
        Tower tower = _container.InstantiatePrefabForComponent<Tower>(towerData.TowerPrefab);
        return tower;
    }

    public void DestroyTower(Tower tower) {
        GameObject.Destroy(tower.gameObject);
    }
}
