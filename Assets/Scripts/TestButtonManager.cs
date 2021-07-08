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

    public TestButtonManager(BuildManager buildManager, IWaveManager waveManager, GameManager gameManager, EnemySpawner enemySpawner):base(buildManager, waveManager, gameManager) {
        Debug.Log("test build manager");
        this.enemySpawner = enemySpawner;
    }

    public new void Initialize() {
        base.Initialize();
        BindButtonsInScene();
    }

    public new void BindButtonsInScene() {
        Button btnSpawn = GameObject.Find("btnSpawnEnemy").GetComponent<Button>();
        btnSpawn.onClick.AddListener(delegate { enemySpawner.SpawnEnemy(EnemyData.Type.Normal); });
    }
}
