namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using System;

    public class UnitTookDamageEventArgs : EventArgs {
        public float DamageAmount { get; private set; }
        public UnitTookDamageEventArgs(float damageAmount) {
            DamageAmount = damageAmount;
        }
    }

    public interface IUnit : Itargetable, IEffectable {

        public event EventHandler<UnitTookDamageEventArgs> OnUnitTookDamage;

        public void ReachedDestination();

        public CharacterData GetCharacterData();
    }
}
