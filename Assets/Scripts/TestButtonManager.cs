using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Adds additional testing functionality to ButtonManager
/// </summary>
public class TestButtonManager : ButtonManager, IInitializable {
    private BuildManager buildManager;
    private IWaveManager waveManager;
    private GameManager gameManager;
    private EnemySpawner enemySpawner;
    private IWallet wallet;

    public TestButtonManager(BuildManager buildManager, IWaveManager waveManager, GameManager gameManager, EnemySpawner enemySpawner, IWallet wallet):base(buildManager, waveManager, gameManager) {
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
        btnSpawnFast.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.Type.Fast); });
        btnSpawnNormal.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.Type.Normal); });
        btnSpawnStrong.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.Type.Strong); });
        btnAddGold.onClick.AddListener(delegate { wallet.GainMoney(100); });
    }
}
