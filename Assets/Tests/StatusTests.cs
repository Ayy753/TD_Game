namespace Tests {

    using DefaultNamespace;
    using DefaultNamespace.EffectSystem;
    using DefaultNamespace.SoundSystem;
    using DefaultNamespace.StatusSystem;
    using NSubstitute;
    using NUnit.Framework;
    using UnityEngine;

    [TestFixture]
    public class StatusTests {
        private CharacterData characterData;

        [SetUp]
        public void Init() {
            characterData = (CharacterData)ScriptableObject.CreateInstance(typeof(CharacterData));
            typeof(CharacterData).GetProperty(nameof(characterData.BaseHealth)).SetValue(characterData, 100);
            typeof(CharacterData).GetProperty(nameof(characterData.BaseArmor)).SetValue(characterData, 20);
            typeof(CharacterData).GetProperty(nameof(characterData.BaseColdResist)).SetValue(characterData, 10);
            typeof(CharacterData).GetProperty(nameof(characterData.BaseFireResist)).SetValue(characterData, 10);
            typeof(CharacterData).GetProperty(nameof(characterData.BaseLightningResist)).SetValue(characterData, 10);
            typeof(CharacterData).GetProperty(nameof(characterData.BasePoisonResist)).SetValue(characterData, 10);
            typeof(CharacterData).GetProperty(nameof(characterData.BaseSpeed)).SetValue(characterData, 10);
        }

        [Test]
        public void Health_after_dealing_5_physical_damage_against_unit_with_20_armor_and_100_health_is_96() {
            IEffectable effectable = CreateMockEffectable(characterData);
            Damage damage = new Damage(5, DamageType.Physical);
            EffectGroup effectGroup = CreateMockEffectGroup(new IEffect[] { damage });

            effectGroup.EffectTarget(effectable);

            Assert.AreEqual(96, effectable.Status.Health.Value);
        }

        [Test]
        public void Speed_Always_Greater_Than_0() {
            IEffectable effectable = CreateMockEffectable(characterData);
            Debuff speedDebuff = new Debuff(10000, 0, StatType.Speed, DamageType.Cold, false);
            EffectGroup effectGroup = CreateMockEffectGroup(new IEffect[] { speedDebuff });

            effectGroup.EffectTarget(effectable);

            Assert.IsTrue(effectable.Status.Speed.Value > 0);
        }

        [Test]
        public void Armor_Cannot_Fall_Below_0() {
            IEffectable effectable = CreateMockEffectable(characterData);
            Debuff armorDebuff = new Debuff(10000, 60, StatType.Armor, DamageType.Physical, false);
            EffectGroup effectGroup = CreateMockEffectGroup(new IEffect[] { armorDebuff });

            effectGroup.EffectTarget(effectable);

            Assert.IsTrue(effectable.Status.Armor.Value >= 0);
        }

        [Test]
        public void Fire_Resist_Cannot_Fall_Below_Negative_75() {
            IEffectable effectable = CreateMockEffectable(characterData);
            Debuff fireResistDebuff = new Debuff(10000, 60, StatType.FireResist, DamageType.Fire, false);
            EffectGroup effectGroup = CreateMockEffectGroup(new IEffect[] { fireResistDebuff });

            effectGroup.EffectTarget(effectable);

            Assert.IsTrue(effectable.Status.FireResist.Value >= -75);
        }

        [Test]
        public void Fire_Resist_Cannot_Exceed_100() {
            IEffectable effectable = CreateMockEffectable(characterData);
            Buff fireResistBuff = new Buff(10000, 60, StatType.FireResist, false);
            EffectGroup effectGroup = CreateMockEffectGroup(new IEffect[] { fireResistBuff });

            effectGroup.EffectTarget(effectable);

            Assert.IsTrue(effectable.Status.FireResist.Value <= 100);
        }

        [Test]
        public void Fire_Damage_Against_100_Fire_Resist_Is_Zero() {
            IEffectable effectable = CreateMockEffectable(characterData);

            //  Raise fire resist to 100%
            Buff fireResistBuff = new Buff(100, 60, StatType.FireResist, false);
            EffectGroup effectGroup = CreateMockEffectGroup(new IEffect[] { fireResistBuff });
            effectGroup.EffectTarget(effectable);

            //  Record health and deal fire damage
            float healthBefore = effectable.Status.Health.Value;
            Damage fireDamage = new Damage(125, DamageType.Fire);
            EffectGroup fireEffect = CreateMockEffectGroup(new IEffect[] { fireDamage });
            fireEffect.EffectTarget(effectable);

            Assert.IsTrue(effectable.Status.Health.Value == healthBefore);
        }

        private IEffectable CreateMockEffectable(CharacterData characterData) {
            Status status = new Status(characterData);
            IEffectable effectable = Substitute.For<IEffectable>();
            effectable.Status = status;
            status.Initialize();

            return effectable;
        }

        private EffectGroup CreateMockEffectGroup(IEffect[] effects) {
            EffectGroup effectGroup = (EffectGroup)ScriptableObject.CreateInstance(typeof(EffectGroup));
            effectGroup.Init(string.Empty, string.Empty, effects, TargetType.Individual, string.Empty, SoundType.arrowHitFlesh, 0, 0);

            return effectGroup;
        }
    }
}
