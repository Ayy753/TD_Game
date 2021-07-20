using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner{
    private ObjectPool objectPool;

    public EnemySpawner(ObjectPool objectPool) {
        this.objectPool = objectPool;
    }

    public Enemy SpawnEnemy(EnemyData.EnemyType type) {
        return objectPool.CreateEnemy(type);
    }
}
