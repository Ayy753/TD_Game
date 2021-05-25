using UnityEngine;
using Zenject;

public class UntitledInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        GameObject[] enemyPrefabs;
        enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");

        Debug.Log("num enemy prefabs " + enemyPrefabs.Length);

        Container.Bind(typeof(IMapManager), typeof(IInitializable)).To<MapManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IPathfinder), typeof(IInitializable)).To<PathFinder>().AsSingle().NonLazy();
        Container.Bind(typeof(ITickable), typeof(IInitializable)).To<ObjectPool>().AsCached().NonLazy();
        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindFactory<UnityEngine.Object, Enemy, Enemy.Factory>().FromFactory<ObjectPool>();
    }
}