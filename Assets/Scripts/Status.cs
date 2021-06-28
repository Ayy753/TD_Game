using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles status effects on an unit
/// </summary>
public class Status{
    private CharacterData characterData;
    private IUnit unit;

    //  Additive modifications to base stats
    //  These can either be positive or negative values and are the result of buffs/debuffs
    private float addHealth;    
    private float addFireResist;
    private float addColdResist;
    private float addPoisonResist;
    private float addLightningResist;
    private float addArmor;
    private float addSpeed;

    //  The amount of damage the unit currently has.
    //  Value will never be negative
    private float damageInflicted;
    
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

    //  Health-related buffs/debuffs increase/decrease max health
    //  Unit dies when current health <= 0
    public float MaxHealth { get { return characterData.BaseHealth + addHealth; } }
    public float CurrentHealth { get { return MaxHealth - DamageInflicted; } }

    public float Speed { get { return characterData.BaseSpeed + addSpeed; } }

    //  Resists/armor are in percentages (value of 100 nullifies all damage of that type, value > 100 heals unit, value below 0 deals additional damage)
    public float FireResist { get { return characterData.BaseFireResist + addFireResist; } }
    public float ColdResist { get { return characterData.BaseColdResist + addColdResist; } }
    public float PoisonResist { get { return characterData.BasePoisonResist + addPoisonResist; } }
    public float LightningResist { get { return characterData.BaseLightningResist + addLightningResist; } }
    public float Armor { get { return characterData.BaseArmor + addArmor; } }

    public List<StatusEffect> statusEffects;

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


    public Status(CharacterData characterData, IUnit unit){
        this.characterData = characterData;
        this.unit = unit;
        statusEffects = new List<StatusEffect>();
    }

    //  Reset status
    public void Initialize() 
    {
        addHealth = 0;
        addFireResist = 0;
        addColdResist = 0;
        addPoisonResist = 0;
        addLightningResist = 0;
        addArmor = 0;
        addSpeed = 0;

        DamageInflicted = 0;
        //  Todo: remove all status effects
    }

    public void ApplyStatusEffect(StatusEffect newEffect)
    {
        foreach (StatusEffect effect in statusEffects)
        {
            if (effect.GetType() == newEffect.GetType())
            {
                effect.StrengthenEffect(newEffect);
            }
        }
        OnStatusChanged.Invoke();
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        statusEffects.Remove(effect);
        OnStatusChanged.Invoke();
    }

    public IEnumerator OnTick()
    {
        foreach (StatusEffect effect in statusEffects)
        {
            effect.OnTick();

            if (effect.RemainingDuration <= 0)
            {
                RemoveStatusEffect(effect);
            }
        }

        yield return new WaitForSeconds(0.3f);
    }

    /// <summary>
    /// Inflicts damage, or heals if damage is negative
    /// </summary>
    /// <param name="damage"></param>
    public void ModifyDamage(float damage) {
        DamageInflicted += damage;

        if (CurrentHealth <= 0) {
            unit.Died();
            if (OnStatusCleared != null) {
                OnStatusCleared.Invoke();
            }
        }
        else {
            if (OnStatusChanged != null) {
                OnStatusChanged.Invoke();
            }
        }
    }

    public IUnit GetUnit() {
        return unit;
    }
}