using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWallet{
    public bool CanAfford(float amount);
    public void GainMoney(float amount);
    public void SpendMoney(float amount);
    public float GetResellPercentageInDecimal();
}
