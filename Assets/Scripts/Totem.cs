using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies positive effects to enemies within range
/// </summary>
public class Totem : MonoBehaviour, IUnitRangeDetection {
    public float Radius { get { return totemData.Radius; } }
    [field: SerializeField] public TotemData totemData { get; private set; }

    private List<IUnit> enemiesInRange;
    private const float buffDelay = 0.33f;
    private RadiusRenderer radiusRenderer;

    private void Start() {
        StartCoroutine(ApplyEffects());
    }

    //  Can't use DI on objects created at runtime
    public void Initialize(RadiusRenderer radiusRenderer) {
        this.radiusRenderer = radiusRenderer;
    }

    private IEnumerator ApplyEffects() {
        while (true) {
            enemiesInRange = GetUnitsInRange(transform.position);
            foreach (IUnit unit in enemiesInRange) {
                unit.GetStatus().ApplyEffectGroup(totemData.EffectGroup);
            }
            yield return new WaitForSeconds(buffDelay);
        }
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

    // ...and the mesh finally turns white when the mouse moves away.
    void OnMouseExit() {
        radiusRenderer.HideRadius();
    }
}
