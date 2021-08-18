namespace DefaultNamespace {
    
    using UnityEngine;

    public class EnemyIcon : MonoBehaviour, IDisplayable {
        public EnemyData enemyData { get; set; }

        public string GetDisplayText() {
            return enemyData.ToString();
        }
    }
}
