using System;
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
        Armor, ColdResist, FireResist, PoisonResist, LightningResist, Health, MaxHealth, Speed, Max
    }
    private float[] statMods = new float[(int)StatType.Max];

    public float Armor { get { return characterData.BaseArmor + statMods[(int)StatType.Armor]; } }
    public float ColdResist { get { return characterData.BaseColdResist + statMods[(int)StatType.ColdResist]; } }
    public float FireResist { get { return characterData.BaseFireResist + statMods[(int)StatType.FireResist]; } }
    public float PoisonResist { get { return characterData.BasePoisonResist + statMods[(int)StatType.PoisonResist]; } }
    public float LightningResist { get { return characterData.BaseLightningResist + statMods[(int)StatType.LightningResist]; } }
    public float CurrentHealth { get { return MaxHealth - DamageInflicted; } }
    public float Speed { get { return characterData.BaseSpeed + statMods[(int)StatType.Speed]; } }
    public float MaxHealth { get { return characterData.BaseHealth + statMods[(int)StatType.Health]; } }

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
        statMods = new float[(int)StatType.Max];
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
        statMods[(int)type] += amount;

        if (OnStatusChanged != null) {
            OnStatusChanged.Invoke();
        }
    }

    private void OnTick() {
        foreach (IStatusEffect statusEffect in statusEffects) {
            statusEffect.OnTick();
            if (statusEffect.RemainingDuration >= 0) {
                statusEffects.Remove(statusEffect);
            }
        }
    }

    //  Reset status
    public void Initialize() {
        //  Clear all stat modifications
        for (int i = 0; i < (int)StatType.Max; i++) {
            statMods[i] = 0;
        }

        DamageInflicted = 0;
        //  Todo: remove all status effects
    }

    public Unit GetUnit() {
        return unit;
    }

    public float GetStat(StatType type) {
        switch (type) {
            case StatType.Armor:
                return Armor;
            case StatType.ColdResist:
                return ColdResist;
            case StatType.FireResist:
                return FireResist;
            case StatType.PoisonResist:
                return PoisonResist;
            case StatType.LightningResist:
                return LightningResist;
            case StatType.Health:
                return CurrentHealth;
            case StatType.MaxHealth:
                return MaxHealth;
            case StatType.Speed:
                return Speed;
            default:
                throw new Exception("Stat type " + type + " is not valid");
        }
    }
}