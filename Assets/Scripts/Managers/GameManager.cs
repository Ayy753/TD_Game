using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {
    public int Lives { get; private set; }
    public int Gold { get; private set; }
    public bool GameEnded { get; internal set; }

    public bool CanAfford(int cost) {
        if (Gold >= cost) {
            return true;
        }
        return false;
    }

    public void SpendGold(int cost) {
        Gold -= cost;
    }

    internal void GainGold(int sellValue) {
        throw new NotImplementedException();
    }
}
