using DefaultNamespace.EffectSystem;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles status effects on an unit
/// </summary>
public class Status : MonoBehaviour {
    [field: SerializeField] public CharacterData characterData { get; private set; }
    private IUnit unit;

    public enum StatType {
        Armor, ColdResist, FireResist, PoisonResist, LightningResist, Health, Speed, Max
    }

    public Stat[] Stats;

    public Resistance Armor { get; private set; }
    public Resistance ColdResist { get; private set; }
    public Resistance FireResist { get; private set; }
    public Resistance PoisonResist { get; private set; }
    public Resistance LightningResist { get; private set; }
    public Health Health { get; private set; }
    public Speed Speed { get; private set; }
    public bool IsDead { get; private set; }

    public List<IStatusEffect> statusEffects;

    public delegate void StatusChanged(StatType statType);
    public delegate void ClearStatus();

    /// <summary>
    /// Instance event StatusPanel subscribes to when unit is targetted
    /// Fires when unit is damaged/healed, or a buff/debuff is applied or removed
    /// </summary>
    public event StatusChanged OnStatusChanged;

    /// <summary>
    /// Instance event StatusPanel subscribes to when unit is targetted
    /// Fires when unit dies and the status panel should be cleared
    /// </summary>
    public event ClearStatus OnStatusCleared;

    private void Awake() {
        unit = transform.GetComponent<IUnit>();

        Armor = new Resistance(characterData.BaseArmor);
        ColdResist = new Resistance(characterData.BaseColdResist);
        FireResist = new Resistance(characterData.BaseFireResist);
        PoisonResist = new Resistance(characterData.BasePoisonResist);
        LightningResist = new Resistance(characterData.BaseLightningResist);
        Health = new Health(characterData.BaseHealth);
        Speed = new Speed(characterData.BaseSpeed);

        Stats = new Stat[] {
            Armor, ColdResist, FireResist, PoisonResist, LightningResist, Health, Speed
        };
    }

    private void OnEnable() {
        TickManager.OnTick += OnTick;

        statusEffects = new List<IStatusEffect>();
        for (int i = 0; i < Stats.Length; i++) {
            Stats[i].Initialize();
        }
        IsDead = false;
    }

    private void OnDisable() {
        TickManager.OnTick -= OnTick;
    }

    public void TakeDamage(float effectiveDamage) {
        if (IsDead == false) {
            Health.TakeDamage(effectiveDamage);

            if (Health.Value <= 0) {
                IsDead = true;
                unit.Died();
            }

            if (OnStatusChanged != null) {
                OnStatusChanged.Invoke(StatType.Health);
            }
        }
    }

    public void RestoreHealth(float amount) {
        Health.Heal(amount);

        if (OnStatusChanged != null) {
            OnStatusChanged.Invoke(StatType.Health);
        }
    }

    public void ModifyStat(StatType type, float amount) {
        Stats[(int)type].ModifyStat(amount);

        if (OnStatusChanged != null) {
            OnStatusChanged.Invoke(type);
        }
    }

    public void ApplyEffectGroup(EffectGroup effectGroup) {
        foreach (IEffect effect in effectGroup.GetEffects()) {
            if (effect is IStatusEffect) {
                statusEffects.Add((IStatusEffect)effect);
            }
            effect.Apply(this);
        }
    }

    private void OnTick() {
        for (int i = 0; i < statusEffects.Count; i++) {
            statusEffects[i].OnTick();
            if (statusEffects[i].RemainingTicks <= 0) {
                statusEffects[i].Remove();
                statusEffects.Remove(statusEffects[i]);
                i--;
            }
        }
    }

    public CharacterData GetCharacterData() {
        return characterData;
    }

    public Stat GetStat(StatType type) {
        return Stats[(int)type];
    }
}