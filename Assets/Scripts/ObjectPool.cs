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

    private readonly GameObject[] towerPrefabs;
    private Dictionary<TowerData.TowerType, GameObject> towerTypeToPrefab;

    private readonly GameObject floatingTextPrefab;
    private List<FloatingText> instantiatedFloatingTexts;

    public ObjectPool(DiContainer container) {
        _container = container;
        enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        projectilePrefabs = Resources.LoadAll<GameObject>("Prefabs/Projectiles");
        towerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Towers");
        floatingTextPrefab = Resources.Load<GameObject>("Prefabs/FloatingText");
    }

    public void Initialize() {
        Debug.Log("Initializing ObjectPool");

        instantiatedEnemies = new List<Enemy>();
        enemyTypeToPrefab = new Dictionary<EnemyData.Type, GameObject>();

        instantiatedProjectiles = new List<Projectile>();
        projectileTypeToPrefab = new Dictionary<ProjectileData.ProjectileType, GameObject>();

        towerTypeToPrefab = new Dictionary<TowerData.TowerType, GameObject>();

        instantiatedFloatingTexts = new List<FloatingText>();

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
                case "SniperProjectile":
                    projectileTypeToPrefab.Add(ProjectileData.ProjectileType.Sniper, prefab);
                    break;
                default:
                    throw new System.Exception(string.Format("Projectile prefab name \"{0}\" does not match any ProjectileData.ProjectileType values", prefab.name));
            }
        }

        for (int i = 0; i < towerPrefabs.Length; i++) {
            GameObject prefab = towerPrefabs[i];
            switch (prefab.name) {
                case "BulletTower":
                    towerTypeToPrefab.Add(TowerData.TowerType.Bullet, prefab);
                    break;
                case "SniperTower":
                    towerTypeToPrefab.Add(TowerData.TowerType.Sniper, prefab);
                    break;
                case "SplashTower":
                    towerTypeToPrefab.Add(TowerData.TowerType.Splash, prefab);
                    break;
                default:
                    throw new System.Exception(string.Format("Tower name \"{0}\" does not match any TowerData.TowerType value", prefab.name));
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
            enemy.transform.gameObject.SetActive(false);
        }

        //  Disable preloaded projectiles
        foreach (Projectile projectile in instantiatedProjectiles) {
            projectile.transform.gameObject.SetActive(false);
        }
    }

    public Enemy CreateEnemy(EnemyData.Type type) {
        //  Find available enemy of a prefab type
        foreach (Enemy enemy in instantiatedEnemies) {
            if (enemy.GetType() == type && enemy.gameObject.activeInHierarchy == false) {
                enemy.transform.gameObject.SetActive(true);
                return enemy;
            }
        }

        //  Instantiate new enemy if none are available in pool
        GameObject prefab = enemyTypeToPrefab[type];
        Enemy newEnemy = _container.InstantiatePrefabForComponent<Enemy>(prefab);

        newEnemy.transform.parent = enemyContainer.transform;
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
    public Tower CreateTower(TowerData.TowerType type) {
        GameObject prefab = towerTypeToPrefab[type];
        Tower tower = _container.InstantiatePrefabForComponent<Tower>(prefab);
        return tower;
    }

    public void DestroyTower(Tower tower) {
        GameObject.Destroy(tower.gameObject);
    }

    public FloatingText CreateFloatingText(Vector3 position, string text, Color color, float textSize = 0.5f) {
        foreach (FloatingText floatingText in instantiatedFloatingTexts) {
            if (floatingText.gameObject.activeInHierarchy == false) {
                floatingText.Initialize(position, text, color, textSize);
                return floatingText;
            }
        }
        FloatingText newFloatingText = GameObject.Instantiate(floatingTextPrefab).GetComponent<FloatingText>();
        newFloatingText.Initialize(position, text, color, textSize);
        instantiatedFloatingTexts.Add(newFloatingText);
        return newFloatingText;
    }
}
