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
    private float AddHealth;    
    private float AddFireResist;
    private float AddColdResist;
    private float AddPoisonResist;
    private float AddLightningResist;
    private float AddArmor;
    private float AddSpeed;

    //  The amount of damage inflicted
    private float Damage;

    //  Effective stats
    //  Resists are in percentage
    public float Health { get { return characterData.BaseHealth + AddHealth - Damage; } }
    public float FireResist { get { return characterData.BaseFireResist + AddFireResist; } }
    public float ColdResist { get { return characterData.BaseColdResist + AddColdResist; } }
    public float PoisonResist { get { return characterData.BasePoisonResist + AddPoisonResist; } }
    public float LightningResist { get { return characterData.BaseLightningResist + AddLightningResist; } }
    public float Armor { get { return characterData.BaseArmor + AddArmor; } }
    public float Speed { get { return characterData.BaseSpeed + AddSpeed; } }

    public List<StatusEffect> statusEffects;

    public Status(CharacterData characterData, IUnit unit){
        this.characterData = characterData;
        this.unit = unit;
        statusEffects = new List<StatusEffect>();
    }

    //  Reset status
    public void Initialize() 
    {
        AddHealth = 0;
        AddFireResist = 0;
        AddColdResist = 0;
        AddPoisonResist = 0;
        AddLightningResist = 0;
        AddArmor = 0;
        AddSpeed = 0;

        Damage = 0;
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
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        statusEffects.Remove(effect);
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
    /// Inflicts damage
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage) {
        Damage += damage;
        if (Health <= 0) {
            unit.Died();
        }
    }

    /// <summary>
    /// Mends inflicted damage
    /// </summary>
    /// <param name="healAmount"></param>
    public void Heal(float healAmount) {
        Damage -= healAmount;
        if (Damage < 0) {
            AddHealth = 0;
        }
    }
}

