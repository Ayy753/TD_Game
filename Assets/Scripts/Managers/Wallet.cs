using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : IWallet {
    private float gold;
    private const float startingGold = 500;

    public const float resellPercentageInDecimal = 0.66f;

    public Wallet() {
        gold = startingGold;
    }

    public bool CanAfford(float amount) {
        if (gold >= amount) {
            return true;
        }
        return false;
    }

    public void GainMoney(float amount) {
        gold += amount;
    }

    public void SpendMoney(float amount) {
        gold -= amount;
    }

    public float GetResellPercentageInDecimal() {
        return resellPercentageInDecimal;
    }
}