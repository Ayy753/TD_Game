using Zenject;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IMapManager>().To<MapManager>().FromScriptableObjectResource("ManagerScriptableObjects/MapManagerScriptableObject").AsSingle();
    }
}
