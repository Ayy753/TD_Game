using System;
using UnityEngine;
using Zenject;

public class Wallet : IWallet, IInitializable, IDisposable {
    [Inject] private IGUIManager guiController;
    [Inject] private IMessageSystem messageSystem;

    private float gold;
    private const float startingGold = 500;
    public const float resellPercentageInDecimal = 0.66f;

    public Wallet(IGUIManager guiController) {
        this.guiController = guiController;
    }

    public void Initialize() {
        GainMoney(startingGold);
        Enemy.OnEnemyDied += HandleEnemyDied;
    }

    public void Dispose() {
        Enemy.OnEnemyDied -= HandleEnemyDied;
    }

    private void HandleEnemyDied(Enemy enemy) {
        float value = enemy.GetValue();
        GainMoney(value);
        messageSystem.DisplayMessageAt(enemy.transform.position, string.Format("+{0}g", value), Color.yellow);
    }

    public bool CanAfford(float amount) {
        if (gold >= amount) {
            return true;
        }
        return false;
    }

    public void GainMoney(float amount) {
        gold += amount;
        guiController.UpdateGoldLabel(gold);
    }

    public void SpendMoney(float amount) {
        gold -= amount;
        guiController.UpdateGoldLabel(gold);
    }

    public float GetResellPercentageInDecimal() {
        return resellPercentageInDecimal;
    }
}