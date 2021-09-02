namespace DefaultNamespace.GUI {

    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class TestButtonManager : ButtonManager, IInitializable {
        private EnemySpawner enemySpawner;
        private IWallet wallet;

        public TestButtonManager(BuildManager buildManager, IWaveManager waveManager, GameManager gameManager, EnemySpawner enemySpawner, IWallet wallet, IGUIManager guiController) : base(buildManager, waveManager, gameManager, guiController) {
            Debug.Log("test build manager");
            this.enemySpawner = enemySpawner;
            this.wallet = wallet;
        }

        public new void Initialize() {
            base.Initialize();
            BindButtonsInScene();
        }

        public new void BindButtonsInScene() {
            Button btnSpawnFast = GameObject.Find("btnSpawnFastEnemy").GetComponent<Button>();
            Button btnSpawnNormal = GameObject.Find("btnSpawnNormalEnemy").GetComponent<Button>();
            Button btnSpawnStrong = GameObject.Find("btnSpawnStrongEnemy").GetComponent<Button>();
            Button btnSpawnGigaCrab = GameObject.Find("btnSpawnGigaCrab").GetComponent<Button>();
            Button btnSpawnTrilobite = GameObject.Find("btnSpawnTrilobite").GetComponent<Button>();
            Button btnAddGold = GameObject.Find("btnAddGold").GetComponent<Button>();
            Button btnAddLife = GameObject.Find("btnAddLife").GetComponent<Button>();
            Button btnRemoveLife = GameObject.Find("btnRemoveLife").GetComponent<Button>();

            btnSpawnFast.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.Fast); });
            btnSpawnNormal.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.Normal); });
            btnSpawnStrong.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.Strong); });
            btnSpawnGigaCrab.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.GigaCrab); });
            btnSpawnTrilobite.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.Trilobite); });
            btnAddGold.onClick.AddListener(delegate { wallet.GainMoney(100); });
            btnAddLife.onClick.AddListener(delegate { gameManager.GainLife(); });
            btnRemoveLife.onClick.AddListener(delegate { gameManager.LoseLife(); });
        }
    }
}
