namespace DefaultNamespace.EffectSystem {

    using DefaultNamespace.StatusSystem;

    public interface IEffectable : ITransform {
        public Status Status { get; set; }
        public void ApplyEffectGroup(EffectGroup effectGroup);
    }
}
