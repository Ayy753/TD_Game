using System;
using UnityEngine;

public class Tower : MonoBehaviour {
    public TowerData TowerData { get; internal set; }

    internal void EnemyEnteredRange(Enemy enemy) {
        throw new NotImplementedException();
    }

    internal void EnemyLeftRange(Enemy enemy) {
        throw new NotImplementedException();
    }
}