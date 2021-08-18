namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using DefaultNamespace.StatusSystem;

    public interface IUnit : Itargetable, IEffectable {
        public void ReachedDestination();

        public void Died();

        public Status GetStatus();
    }
}
