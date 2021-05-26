using UnityEngine;
using Zenject;

public class UntitledInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind(typeof(IMapManager), typeof(IInitializable)).To<MapManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IPathfinder), typeof(IInitializable)).To<PathFinder>().AsSingle().NonLazy();



        //Container.Bind(typeof(ITickable), typeof(IInitializable)).To<ObjectPool>().AsSingle().NonLazy();
        Container.Bind<ObjectPool>().AsSingle().NonLazy();
        Container.Bind(typeof(ITickable), typeof(IInitializable)).To<ObjectPool>().FromResolve();


        Container.BindIFactory<Enemy, Enemy.Factory>();
        Container.Bind(typeof(IInitializable)).To<WaveManager>().AsSingle().NonLazy();
        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle();
    }
}