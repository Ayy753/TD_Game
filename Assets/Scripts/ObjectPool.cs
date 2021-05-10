using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private List<Enemy> enemyPool;
    private List<Projectile> projectilePool;
    private List<FloatingText> floatingTextPool;

    //  Used to keep scene hierarchy organized
    Transform projectileContainer;
    Transform textContainer;
    Transform enemyContainer;

    [SerializeField]
    List<GameObject> enemyPrefabs;

    [SerializeField]
    private GameObject floatingTextPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        enemyPool = new List<Enemy>();
        projectilePool = new List<Projectile>();
        floatingTextPool = new List<FloatingText>();

        projectileContainer = transform.Find("ProjectilePool");
        textContainer = transform.Find("TextPool");
        enemyContainer = transform.Find("EnemyPool");

        PreloadPool();
    }

    /// <summary>
    /// Instantiates objects into pool ahead of time to avoid fps drops
    /// at the start of level
    /// </summary>
    private void PreloadPool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newTextGO = Instantiate(floatingTextPrefab, textContainer);
            FloatingText newText = newTextGO.GetComponent<FloatingText>();
            floatingTextPool.Add(newText);
            newTextGO.SetActive(false);
        }

        foreach (GameObject prefabType in enemyPrefabs)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject newEnemy = Instantiate(prefabType, enemyContainer);
                Enemy newEnemyObj = newEnemy.GetComponentInChildren<Enemy>();
                enemyPool.Add(newEnemyObj);
                newEnemy.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Returns new enemy based on enemyData by either finding 
    /// an inactive one of this type or instantiating a new one
    /// </summary>
    /// <param name="enemyData"></param>
    /// <returns></returns>
    public Enemy CreateEnemy(EnemyData enemyData) 
    {
        foreach (Enemy enemy in enemyPool)
        {
            if (enemy.EnemyData == enemyData && enemy.gameObject.activeInHierarchy == false)
            {
                enemy.gameObject.SetActive(true);
                return enemy;
            }
        }

        //  If no enemy found, instantiate a new game object
        GameObject newEnemy = Instantiate(enemyData.Prefab, enemyContainer);
        Enemy newEnemyObj = newEnemy.GetComponentInChildren<Enemy>();
        enemyPool.Add(newEnemyObj);
        return newEnemyObj;
    }

    public Projectile CreateProjectile(GameObject projectilePrefab, Vector3 position)
    {
        Projectile projectileType = projectilePrefab.GetComponent<Projectile>();

        foreach (Projectile projectile in projectilePool)
        {
            if (projectileType.GetType() == projectile.GetType() && projectile.gameObject.activeInHierarchy == false)
            {
                projectile.gameObject.SetActive(true);
                projectile.gameObject.transform.position = position;
                return projectile;
            }
        }

        GameObject newProjectileGO = Instantiate(projectilePrefab, position, new Quaternion(), projectileContainer);
        Projectile newProjectile = newProjectileGO.GetComponent<Projectile>();
        projectilePool.Add(newProjectile);
        return newProjectile;
    }

    /// <summary>
    /// Returns a floating text object by either finding an inactive
    /// object from pool or instantiating a new one
    /// </summary>
    /// <returns></returns>
    public FloatingText CreateFloatingText()
    {
        foreach (FloatingText text in floatingTextPool)
        {
            if (text.gameObject.activeInHierarchy == false)
            {
                text.gameObject.SetActive(true);
                return text;
            }
        }

        GameObject newTextGO = Instantiate(floatingTextPrefab, textContainer);
        FloatingText newText = newTextGO.GetComponent<FloatingText>();
        floatingTextPool.Add(newText);
        return newText;
    }
}
