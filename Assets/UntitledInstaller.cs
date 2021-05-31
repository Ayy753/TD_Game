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
        Container.Bind(typeof(ITickable), typeof(IInitializable)).To<ObjectPool>().FromResolve();
        Container.BindIFactory<Enemy, Enemy.Factory>();
        Container.Bind(typeof(IInitializable)).To<WaveManager>().AsSingle().NonLazy();
        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle();

        Container.Bind<MouseManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind<GUIController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<GameManager>().AsSingle().NonLazy();

        Container.Bind<BuildManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<BuildManager>().FromResolve();

        Container.Bind<HoverManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<HoverManager>().FromResolve();

        //StructureData[] structureDatas = Resources.LoadAll<StructureData>("ScriptableObjects/TileData/StructureData");

        //for (int i = 0; i < structureDatas.Length; i++) {
        //    Container.BindInstance(structureDatas[i]).AsTransient();
        //    Container.InstantiatePrefabResourceForComponent<BuildMenuButton>("Prefabs/NewBuildMenuButton");
        //}
    }
}