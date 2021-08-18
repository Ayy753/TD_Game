namespace DefaultNamespace {

    public class EnemySpawner {
        private ObjectPool objectPool;

        public EnemySpawner(ObjectPool objectPool) {
            this.objectPool = objectPool;
        }

        public Enemy SpawnEnemy(EnemyData.EnemyType type) {
            return objectPool.CreateEnemy(type);
        }
    }
}
