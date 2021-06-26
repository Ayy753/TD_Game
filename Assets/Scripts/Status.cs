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

    private float[] stats = new float[(int)StatType.Max];
    private List<IStatusEffect> statusEffects;

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

    public void Initialize() {
        stats[(int)StatType.Armor] = characterData.BaseArmor;
        stats[(int)StatType.ColdResist] = characterData.BaseColdResist;
        stats[(int)StatType.FireResist] = characterData.BaseFireResist;
        stats[(int)StatType.Health] = characterData.BaseHealth;
        stats[(int)StatType.MaxHealth] = characterData.BaseHealth;
        stats[(int)StatType.LightningResist] = characterData.BaseLightningResist;
        stats[(int)StatType.PoisonResist] = characterData.BasePoisonResist;
        stats[(int)StatType.Speed] = characterData.BaseSpeed;
    }

    public void TakeDamage(float amount) {
        ModifyStat(StatType.Health, -amount);

        if (GetStat(StatType.Health) <= 0) {
            unit.Died();
        }
    }

    public void RestoreHealth(float amount) {
        ModifyStat(StatType.Health, amount);
    }

    /// <summary>
    /// Returns value of specified stat
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetStat(StatType type) {
        return stats[(int)type];
    }

    /// <summary>
    /// Addsa/subtracts amount from specified stat
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public void ModifyStat(StatType type, float amount) {
        stats[(int)type] += amount;

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

    public Unit GetUnit() {
        return unit;
    }
}