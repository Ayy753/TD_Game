using System;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, IUnit {
    [Inject] private IMessageSystem messageSystem;
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
        
    public void Spawn() {
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

    //public override void ApplyDamage(Damage.DamageTypeAndAmount[] damages) {
    //    float damage = Damage.CalculateDamage(status, damages);
    //    status.ModifyDamage(damage);
    //    messageSystem.DisplayMessageAt(transform.position, Math.Round(damage, 1).ToString(), Color.red, 0.33f);
    //    healthBar.UpdateHealthBar();
    //}

    public Transform GetTransform() {
        return transform;
    }
    
    public float GetValue() {
        return ((EnemyData)GetStatus().GetCharacterData()).BaseValue;
    }

    public new EnemyData.Type GetType() {
        return ((EnemyData)GetStatus().GetCharacterData()).MyType;
    }

    public string GetDescription() {
        return ((EnemyData)status.characterData).Description;
    }

    public string GetName() {
        return ((EnemyData)status.characterData).Name;
    }

    public void ApplyDamage(float damage) {
        status.TakeDamage(damage);
    }

    public class Factory : PlaceholderFactory<EnemyData.Type, Enemy> { }
}
