using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles status effects on an unit
/// </summary>
public class Status{
    private Unit unit;
    private CharacterData characterData;

    public enum StatType {
        Armor, ColdResist, FireResist, PoisonResist, LightningResist, Health, Speed, Max
    }
    public float[] StatMods = new float[(int)StatType.Max];
    
    public float MaxHealth { get { return characterData.BaseHealth + StatMods[(int)StatType.Health]; } }
    public float CurrentHealth { get { return MaxHealth - DamageInflicted; } }
    public float Speed { get { return characterData.BaseSpeed + StatMods[(int)StatType.Speed]; } }
    public float FireResist { get { return characterData.BaseFireResist + StatMods[(int)StatType.FireResist]; } }
    public float ColdResist { get { return characterData.BaseColdResist + StatMods[(int)StatType.ColdResist]; } }
    public float PoisonResist { get { return characterData.BasePoisonResist + StatMods[(int)StatType.PoisonResist]; } }
    public float LightningResist { get { return characterData.BaseLightningResist + StatMods[(int)StatType.LightningResist]; } }
    public float Armor { get { return characterData.BaseArmor + StatMods[(int)StatType.Armor]; } }

    /// <summary>
    /// The current amount of damage inflicted on unit.
    /// Dealing negative damage heals the unit. 
    /// The total inflicted damage will always be >= 0
    /// </summary>
    public float DamageInflicted {
        get {
            return damageInflicted;
        }
        private set {
            damageInflicted = value;
            if (damageInflicted < 0) {
                damageInflicted = 0;
            }
        }
    }

    //  The amount of damage the unit currently has. Value will never be negative
    private float damageInflicted;

    public List<IStatusEffect> statusEffects;

    public delegate void StatusChanged();
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

    public Status(CharacterData characterData, Unit unit) {
        this.characterData = characterData;
        this.unit = unit;
        statusEffects = new List<IStatusEffect>();
    }

    public void TakeDamage(float effectiveDamage) {
        DamageInflicted += effectiveDamage;

        if (CurrentHealth <= 0) {
            unit.Died();
        }
    }

    public void RestoreHealth(float amount) {
        DamageInflicted -= amount;
    }

    public void ModifyStat(StatType type, float amount) {
        StatMods[(int)type] += amount;
    }

    public void ApplyStatusEffect(IStatusEffect statusEffect) {
        statusEffects.Add(statusEffect);
    }

    //  Reset status
    public void Initialize() {
        //  Clear all stat modifications
        for (int i = 0; i < (int)StatType.Max; i++) {
            StatMods[i] = 0;
        }

        DamageInflicted = 0;
        //  Todo: remove all status effects
    }

    //public void ApplyStatusEffect(StatusEffect newEffect)
    //{
    //    foreach (StatusEffect effect in statusEffects)
    //    {
    //        if (effect.GetType() == newEffect.GetType())
    //        {
    //            effect.StrengthenEffect(newEffect);
    //        }
    //    }
    //    OnStatusChanged.Invoke();
    //}

    //public void RemoveStatusEffect(StatusEffect effect)
    //{
    //    statusEffects.Remove(effect);
    //    OnStatusChanged.Invoke();
    //}

    //public IEnumerator OnTick()
    //{
    //    foreach (StatusEffect effect in statusEffects)
    //    {
    //        effect.OnTick();

    //        if (effect.RemainingDuration <= 0)
    //        {
    //            RemoveStatusEffect(effect);
    //        }
    //    }

    //    yield return new WaitForSeconds(0.3f);
    //}

    //public Unit GetUnit() {
    //    return unit;
    //}
}