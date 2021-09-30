namespace DefaultNamespace {

    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class EnemySpawner : IInitializable {
        private readonly ObjectPool objectPool;
        private Dictionary<int, EnemyData> enemyIdToEnemyData;
        
        public void Initialize() {
            enemyIdToEnemyData = new Dictionary<int, EnemyData>();
            EnemyData[] enemyDatas = Resources.LoadAll<EnemyData>("ScriptableObjects/EnemyData");

            foreach (EnemyData enemyData in enemyDatas) {
                enemyIdToEnemyData.Add(enemyData.EnemyId, enemyData);
            }
        }

        public EnemySpawner(ObjectPool objectPool) {
            this.objectPool = objectPool;
        }

        public Enemy SpawnEnemy(int type) {
            return objectPool.CreateEnemy(type);
        }

        public Dictionary<int, EnemyData> EnemyIdToEnemyData() {
            return enemyIdToEnemyData;
        }
    }
}
