using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status{
    private CharacterData characterData;

    private float AddHealth;    
    private float AddFireResist;
    private float AddColdResist;
    private float AddPoisonResist;
    private float AddLightningResist;
    private float AddArmor;
    private float AddSpeed;

    public float Health { get { return characterData.BaseHealth + AddHealth; } }
    public float FireResist { get { return characterData.BaseFireResist + AddFireResist; } }
    public float ColdResist { get { return characterData.BaseColdResist + AddColdResist; } }
    public float PoisonResist { get { return characterData.BasePoisonResist + AddPoisonResist; } }
    public float LightningResist { get { return characterData.BaseLightningResist + AddLightningResist; } }
    public float Armor { get { return characterData.BaseArmor + AddArmor; } }
    public float Speed { get { return characterData.BaseSpeed + AddSpeed; } }

    public List<StatusEffect> statusEffects;

    public Status(CharacterData characterData)
    {
        this.characterData = characterData;
        statusEffects = new List<StatusEffect>();
    }

    public void Initialize() {
        //  TODO: Remove each status effect
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
}

