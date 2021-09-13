namespace Tests {

    using DefaultNamespace;
    using DefaultNamespace.EffectSystem;
    using DefaultNamespace.SoundSystem;
    using DefaultNamespace.StatusSystem;
    using NSubstitute;
    using NUnit.Framework;
    using UnityEngine;

    public class StatusTests {
        [Test]
        public void Health_after_dealing_5_physical_damage_against_unit_with_20_armor_and_100_health_is_96() {
            CharacterData characterData = (CharacterData)ScriptableObject.CreateInstance(typeof(CharacterData));
            typeof(CharacterData).GetProperty(nameof(characterData.BaseArmor)).SetValue(characterData, 20);
            typeof(CharacterData).GetProperty(nameof(characterData.BaseHealth)).SetValue(characterData, 100);
            IEffectable effectable = CreateMockEffectable(characterData);
            Damage damage = new Damage(5, DamageType.Physical);
            EffectGroup effectGroup = CreateMockEffectGroup(new IEffect[] { damage });

            effectGroup.EffectTarget(effectable);
            
            Assert.AreEqual(96, effectable.Status.Health.Value);
        }

        private IEffectable CreateMockEffectable(CharacterData characterData) {
            Status status = new Status(characterData);
            IEffectable effectable = Substitute.For<IEffectable>();
            effectable.Status = status;

            return effectable;
        }

        private EffectGroup CreateMockEffectGroup(IEffect[] effects) {
            EffectGroup effectGroup = (EffectGroup)ScriptableObject.CreateInstance(typeof(EffectGroup));
            effectGroup.Init(string.Empty, string.Empty, effects, TargetType.Individual, string.Empty, SoundType.arrowHitFlesh, 0, 0);

            return effectGroup;
        }
    }
}
