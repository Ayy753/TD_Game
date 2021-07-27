using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Adds additional testing functionality to ButtonManager
/// </summary>
public class TestButtonManager : ButtonManager, IInitializable {
    private EnemySpawner enemySpawner;
    private IWallet wallet;

    public TestButtonManager(BuildManager buildManager, IWaveManager waveManager, GameManager gameManager, EnemySpawner enemySpawner, IWallet wallet, WaveReportPanel waveReportPanel) :base(buildManager, waveManager, gameManager, waveReportPanel) {
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
        Button btnAddGold = GameObject.Find("btnAddGold").GetComponent<Button>();
        btnSpawnFast.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.Fast); });
        btnSpawnNormal.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.Normal); });
        btnSpawnStrong.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.EnemyType.Strong); });
        btnAddGold.onClick.AddListener(delegate { wallet.GainMoney(100); });
    }
}
