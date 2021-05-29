using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class GUIController : MonoBehaviour {

    [Inject] BuildManager buildManager;
    
    internal void SpawnFloatingText(Vector3 vector3, string v, Color yellow) {
        throw new NotImplementedException();
    }

    internal void SpawnFloatingTextAtCursor(string v, Color red) {
        throw new NotImplementedException();
    }

    internal void SpawnFloatingTextInCenter(string v, Color yellow) {
        throw new NotImplementedException();
    }


    public void EnterBuildMode() {
        buildManager.EnterBuildMode(new TowerData());
    }

    public void ExitBuildMode() {
        buildManager.ExitBuildMode();
    }

}
