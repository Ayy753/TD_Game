using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies positive effects to enemies within range
/// </summary>
public class Totem : MonoBehaviour, IUnitRangeDetection, Itargetable {
    public float Radius { get { return totemData.Radius; } }
    [field: SerializeField] public TotemData totemData { get; private set; }

    private List<IUnit> enemiesInRange;
    private RadiusRenderer radiusRenderer;

    public event EventHandler TargetDisabled;
    private int numberOfTicksPerCooldown;
    private int tickCounter;

    private void OnEnable() {
        TickManager.OnTick += HandleTick;
        numberOfTicksPerCooldown = (int)(totemData.EffectDelay / TickManager.tickFrequency);
        tickCounter = 0;
    }

    private void OnDisable() {
        TickManager.OnTick -= HandleTick;
    }

    private void HandleTick() {
        tickCounter++;
        if (tickCounter == numberOfTicksPerCooldown) {
            ApplyEffects();
            tickCounter = 0;
        }
    }

    private void ApplyEffects() {
        enemiesInRange = GetUnitsInRange(transform.position);
        foreach (IUnit unit in enemiesInRange) {
            unit.GetStatus().ApplyEffectGroup(totemData.EffectGroup);
        }
    }

    //  Can't use DI on objects created at runtime
    public void Initialize(RadiusRenderer radiusRenderer) {
        this.radiusRenderer = radiusRenderer;
    }

    public List<IUnit> GetUnitsInRange(Vector3 center) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, Radius);
        enemiesInRange = new List<IUnit>();

        foreach (var collider in colliders) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) {
                enemiesInRange.Add(enemy);
            }
        }
        return enemiesInRange;
    }

    void OnMouseOver() {
        radiusRenderer.RenderRadius(transform.position, Radius);
    }

    void OnMouseExit() {
        radiusRenderer.HideRadius();
    }

    public Transform GetTransform() {
        return transform;
    }

    public string GetName() {
        return totemData.Name;
    }

    public string GetDescription() {
        return totemData.ToString();
    }
}
