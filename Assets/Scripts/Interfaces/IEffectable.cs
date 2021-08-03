public interface IEffectable : ITransform{
    public void ApplyEffectGroup(EffectGroup effectGroup);
    public Status GetStatus();
}
