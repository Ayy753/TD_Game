using System;
using UnityEngine;
using Zenject;

public class LevelSelectInstaller : MonoInstaller {
    public override void InstallBindings() {
        Container.Bind<LevelManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<LevelManager>().FromResolve();
    }
}