using System;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, IUnit {
    private Status status;
    private HealthBar healthBar;

    public delegate void EnemyReachedGate(Enemy enemy);
    public delegate void EnemyDied(Enemy enemy);

    public static event EnemyReachedGate OnEnemyReachedGate;
    public static event EnemyDied OnEnemyDied;
    public event EventHandler TargetDisabled;

    private void Awake() {
        healthBar = transform.parent.GetComponentInChildren<HealthBar>();
        status = transform.GetComponent<Status>();
    }

    private void OnEnable() {
        healthBar.Initialize(status);
    }

    public void ReachedDestination() {
        Despawn();
        if (OnEnemyReachedGate != null) {
            OnEnemyReachedGate.Invoke(this);
        }
    }

    public void Died() {
        Despawn();
        if (OnEnemyDied != null) {
            OnEnemyDied.Invoke(this);
        }
    }

    private void Despawn() {
        if (TargetDisabled != null) {
            TargetDisabled.Invoke(this, EventArgs.Empty);
        }
        transform.parent.gameObject.SetActive(false);
    }

    public Status GetStatus() {
        return status;
    }

    public Transform GetTransform() {
        return transform;
    }
    
    public float GetValue() {
        return ((EnemyData)GetStatus().GetCharacterData()).BaseValue;
    }

    public EnemyData.EnemyType GetEnemyType() {
        return ((EnemyData)GetStatus().GetCharacterData()).Type;
    }

    public string GetDescription() {
        return ((EnemyData)status.characterData).Description;
    }

    public string GetName() {
        return ((EnemyData)status.characterData).Name;
    }

    public void ApplyEffectGroup(EffectGroup effectGroup) {
        status.ApplyEffectGroup(effectGroup);
    }

    public class Factory : PlaceholderFactory<EnemyData.EnemyType, Enemy> { }
}
