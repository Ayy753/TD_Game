using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Wallet : IWallet, IInitializable {
    [Inject] private IGUIManager guiController;

    private float gold;
    private const float startingGold = 500;
    public const float resellPercentageInDecimal = 0.66f;

    public Wallet(IGUIManager guiController) {
        this.guiController = guiController;
    }

    public void Initialize() {
        GainMoney(startingGold);
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