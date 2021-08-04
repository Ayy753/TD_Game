using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies positive effects to enemies within range
/// </summary>
public class Totem : MonoBehaviour, IEffectableRangeDetection, Itargetable {
    public float Radius { get { return totemData.EffectGroup.Radius; } }
    [field: SerializeField] public TotemData totemData { get; private set; }

    private List<IEffectable> effectableOjbectsInRange;
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
        effectableOjbectsInRange = GetEffectableObjectsInRange(transform.position);
        foreach (IEffectable effectable in effectableOjbectsInRange) {
            totemData.EffectGroup.EffectArea(transform.position);
        }
    }

    //  Can't use DI on objects created at runtime
    public void Initialize(RadiusRenderer radiusRenderer) {
        this.radiusRenderer = radiusRenderer;
    }

    public List<IEffectable> GetEffectableObjectsInRange(Vector3 center) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, Radius);
        List<IEffectable> EffectableObjectsInRange = new List<IEffectable>();

        foreach (var collider in colliders) {
            IEffectable effectable = collider.GetComponent<IEffectable>();
            if (effectable != null) {
                EffectableObjectsInRange.Add(effectable);
            }
        }

        return EffectableObjectsInRange;
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
