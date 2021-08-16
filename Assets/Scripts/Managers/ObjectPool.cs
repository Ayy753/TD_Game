using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
public class ObjectPool : IInitializable, IDisposable{
    EffectParserJSON effectParser;

    private readonly DiContainer _container;
    private List<Enemy> instantiatedEnemies;
    private GameObject enemyContainer;
    private Dictionary<EnemyData.EnemyType, GameObject> enemyTypeToPrefab;

    private readonly GameObject projectilePrefab;
    private List<Projectile> instantiatedProjectiles;
    private GameObject projectileContainer;

    private Dictionary<TowerData.TowerType, GameObject> towerTypeToPrefab;

    private readonly GameObject floatingTextPrefab;
    private List<FloatingText> instantiatedFloatingTexts;

    public ObjectPool(DiContainer container, EffectParserJSON effectParser) {
        _container = container;
        this.effectParser = effectParser;

        projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Projectile");
        floatingTextPrefab = Resources.Load<GameObject>("Prefabs/FloatingText");
    }

    public void Initialize() {
        Debug.Log("Initializing ObjectPool");

        instantiatedProjectiles = new List<Projectile>();
        instantiatedFloatingTexts = new List<FloatingText>();
        instantiatedEnemies = new List<Enemy>();

        InitializeEnemies();
        InitializeTowers();

        //  Scene hierarchy container for new enemies
        enemyContainer = new GameObject("Enemy Container");
        projectileContainer = new GameObject("Projectile Container");

        Tower.OnProjectileFired += Tower_OnProjectileFired;
    }

    public void Dispose() {
        Tower.OnProjectileFired -= Tower_OnProjectileFired;
    }

    private void InitializeEnemies() {
        GameObject[] enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        enemyTypeToPrefab = new Dictionary<EnemyData.EnemyType, GameObject>();

        //  Link enemy types to prefabs
        for (int i = 0; i < enemyPrefabs.Length; i++) {
            GameObject prefab = enemyPrefabs[i];
            switch (prefab.name) {
                case "BaseEnemy":
                    //  Ignore this because it does not get instantiated
                    break;
                case "FastEnemy Variant":
                    enemyTypeToPrefab.Add(EnemyData.EnemyType.Fast, prefab);
                    break;
                case "NormalEnemy Variant":
                    enemyTypeToPrefab.Add(EnemyData.EnemyType.Normal, prefab);
                    break;
                case "StrongEnemy Variant":
                    enemyTypeToPrefab.Add(EnemyData.EnemyType.Strong, prefab);
                    break;
                default:
                    throw new System.Exception(string.Format("Enemy prefab name \"{0}\" does not match any Enemy.Type values ", prefab.name));
            }
        }
    }

    private void InitializeTowers() {
        GameObject[] towerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Towers");
        towerTypeToPrefab = new Dictionary<TowerData.TowerType, GameObject>();

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
                case "FrostTower":
                    towerTypeToPrefab.Add(TowerData.TowerType.Frost, prefab);
                    break;
                case "PoisonTower":
                    towerTypeToPrefab.Add(TowerData.TowerType.Poison, prefab);
                    break;
                default:
                    throw new System.Exception(string.Format("Tower name \"{0}\" does not match any TowerData.TowerType value", prefab.name));
            }

            //  Set effect group for this towerdata object
            TowerData towerData = prefab.GetComponent<Tower>().TowerData;
            EffectGroup effectGroup = effectParser.GetEffectGroup(towerData.ProjectileName);
            towerData.SetEffectGroup(effectGroup);
        }
    }

    private void Tower_OnProjectileFired(object sender, Tower.ProjectileFiredEventArgs e) {
        Projectile projectile = CreateProjectile();
        projectile.Initialize(e.Position, e.Target, e.EffectGroup);
    }

    public Enemy CreateEnemy(EnemyData.EnemyType type) {
        //  Find available enemy of a prefab type
        foreach (Enemy enemy in instantiatedEnemies) {
            if (enemy.GetEnemyType() == type && enemy.gameObject.activeInHierarchy == false) {
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

    public Projectile CreateProjectile() {
        foreach (Projectile projectile in instantiatedProjectiles) {
            if (projectile.gameObject.activeInHierarchy == false) {
                projectile.gameObject.SetActive(true);
                return projectile;
            }
        }

        //  instanteiate new projectile if none are available in pool
        Projectile newProjectile = _container.InstantiatePrefabForComponent<Projectile>(projectilePrefab);

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
