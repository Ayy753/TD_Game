using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles status effects on an unit
/// </summary>
public class Status : MonoBehaviour {
    [field: SerializeField] public CharacterData characterData { get; private set; }
    private IUnit unit;
    private HealthBar healthBar;

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

    private void Awake() {
        unit = transform.GetComponent<IUnit>();
        healthBar = transform.parent.GetComponentInChildren<HealthBar>();

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

        for (int i = 0; i < (int)StatType.Max; i++) {
            Debug.Log(Stats[i]);
        }

    }

    private void OnEnable() {
        statusEffects = new List<IStatusEffect>();
        for (int i = 0; i < Stats.Length; i++) {
            Stats[i].Initialize();
        }
    }

    public void TakeDamage(float effectiveDamage) {
        Health.TakeDamage(effectiveDamage);
        healthBar.UpdateHealthBar();

        if (Health.Value <= 0) {
            unit.Died();
        }

        if (OnStatusChanged != null) {
            OnStatusChanged.Invoke();
        }
    }

    public void RestoreHealth(float amount) {
        Health.Heal(amount);
        healthBar.UpdateHealthBar();

        if (OnStatusChanged != null) {
            OnStatusChanged.Invoke();
        }
    }

    public void ModifyStat(StatType type, float amount) {
        Stats[(int)type].ModifyStat(amount);

        if (OnStatusChanged != null) {
            OnStatusChanged.Invoke();
        }

        if (type == StatType.Health) {
            healthBar.UpdateHealthBar();
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

    public CharacterData GetCharacterData() {
        return characterData;
    }

    public Stat GetStat(StatType type) {
        Debug.Log("getting type " + type);
        return Stats[(int)type];
    }
}