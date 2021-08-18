namespace DefaultNamespace.GUI {
    
    using Zenject;

    public class LevelSelectInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelSelectionPanel>().AsSingle().NonLazy();
        }
    }
}
