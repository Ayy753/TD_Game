using System;
using Zenject;

public class UntitledInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AsyncProcessor>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind(typeof(IMapManager), typeof(IInitializable)).To<MapManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IPathfinder), typeof(IInitializable), typeof(IDisposable)).To<PathFinder>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ObjectPool>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<ObjectPool>().FromResolve();
        Container.BindIFactory<Enemy, Enemy.Factory>();
        Container.Bind(typeof(IButtonManager), typeof(IInitializable)).To<ButtonManager>().AsSingle();
        Container.Bind(typeof(IGUIManager), typeof(IInitializable)).To<GUIController>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<WaveManager>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<BuildManager>().AsSingle().NonLazy();


        Container.Bind<MouseManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind<GameManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<GameManager>().FromResolve();

        Container.Bind<HoverManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<HoverManager>().FromResolve();

        Container.Bind(typeof(IBuildValidator)).To<BuildValidator>().AsSingle().NonLazy();

        Container.Bind(typeof(IWallet), typeof(IInitializable), typeof(IDisposable)).To<Wallet>().AsSingle().NonLazy();

        Container.Bind<IMessageSystem>().To<MessageSystem>().AsSingle().NonLazy();

        Container.BindFactory<Tower, Tower.Factory>();


        Container.Bind<RadiusRenderer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PathRenderer>().FromComponentInHierarchy().AsSingle();

        Container.Bind<StatusPanel>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<StatusPanel>().FromResolve();

        Container.Bind<TowerPanel>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<TowerPanel>().FromResolve();

        Container.Bind<TargetManager>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable), typeof(IDisposable)).To<TargetManager>().FromResolve();

        Container.Bind<ToolTip>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<TooltipManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind<InputHandler>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind<FPSCounter>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<FPSCounter>().FromResolve();

        Container.Bind<ControlsPanel>().AsSingle().NonLazy();
        Container.Bind(typeof(IInitializable)).To<ControlsPanel>().FromResolve();

        Container.Bind<EffectParserJSON>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}