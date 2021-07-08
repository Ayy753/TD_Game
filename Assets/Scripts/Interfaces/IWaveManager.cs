using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWaveManager {
    public int NumberOfWaves { get; }
    public void StartNextWave();
}
