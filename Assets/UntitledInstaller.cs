using System;
using UnityEngine;
using Zenject;

public class UntitledInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind(typeof(IMapManager), typeof(IInitializable)).To<MapManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IPathfinder), typeof(IInitializable)).To<PathFinder>().AsSingle().NonLazy();
        Container.Bind<ObjectPool>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<ObjectPool>().FromResolve();
        Container.BindIFactory<Enemy, Enemy.Factory>();

        Container.Bind<WaveManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<WaveManager>().FromResolve();

        Container.Bind<BuildManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<BuildManager>().FromResolve();

        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle();

        Container.Bind<MouseManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind<GameManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<GameManager>().FromResolve();

        Container.Bind<HoverManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<HoverManager>().FromResolve();

        Container.Bind(typeof(IBuildValidator)).To<BuildValidator>().AsSingle().NonLazy();

        Container.Bind(typeof(IWallet), typeof(IInitializable), typeof(IDisposable)).To<Wallet>().AsSingle().NonLazy();

        Container.Bind<IMessageSystem>().To<MessageSystem>().AsSingle().NonLazy();

        Container.BindFactory<Tower, Tower.Factory>();

        Container.Bind(typeof(IGUIManager)).To<GUIController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<RadiusRenderer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PathRenderer>().FromComponentInHierarchy().AsSingle();

        Container.Bind<StatusPanel>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<StatusPanel>().FromResolve();

        Container.Bind<TowerPanel>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<TowerPanel>().FromResolve();

        Container.Bind<TargetManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<TargetManager>().FromResolve();
    }
}