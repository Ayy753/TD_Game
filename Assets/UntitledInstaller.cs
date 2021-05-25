using UnityEngine;
using Zenject;

public class UntitledInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind(typeof(IMapManager), typeof(IInitializable)).To<MapManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IPathfinder), typeof(IInitializable)).To<PathFinder>().AsSingle().NonLazy();
        Container.Bind(typeof(ITickable), typeof(IInitializable)).To<ObjectPool>().AsCached().NonLazy();
        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindFactory<Enemy.Type, Enemy, Enemy.Factory>().FromFactory<ObjectPool>();
    }
}