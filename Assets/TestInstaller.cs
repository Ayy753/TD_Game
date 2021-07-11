using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller {
    public override void InstallBindings() {

        //  Manager bindings
        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MapManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PathFinder>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ObjectPool>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TestButtonManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GUIController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TestWaveManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BuildManager>().AsSingle().NonLazy();
        Container.Bind<MouseManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<HoverManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<BuildValidator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<Wallet>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MessageSystem>().AsSingle().NonLazy();
        Container.Bind<EffectParserJSON>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TooltipManager>().AsSingle().NonLazy();
        Container.Bind<InputHandler>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        //  Misc bindings
        Container.Bind<RadiusRenderer>().FromNewComponentOnNewPrefabResource("Prefabs/RadiusRenderer").AsSingle();
        Container.Bind<MainPathRenderer>().FromNewComponentOnNewPrefabResource("Prefabs/MainPathRenderer").AsSingle().NonLazy();
        Container.Bind<PreviewPathRenderer>().FromNewComponentOnNewPrefabResource("Prefabs/PreviewPathRenderer").AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<StatusPanel>().AsSingle();
        Container.BindInterfacesAndSelfTo<TowerPanel>().AsSingle();
        Container.BindInterfacesAndSelfTo<TargetManager>().AsSingle();
        Container.Bind<ToolTip>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<FPSCounter>().AsSingle();
        Container.BindInterfacesAndSelfTo<ControlsPanel>().AsSingle();

        //  Factory bindings
        Container.BindIFactory<Enemy, Enemy.Factory>();
        Container.BindFactory<Tower, Tower.Factory>();
    }
}