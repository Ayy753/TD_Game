using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class Totem : MonoBehaviour, IUnitRangeDetection {
    [Inject] EffectParserJSON effectParser;
    [Inject] RadiusRenderer radiusRenderer;
    public float Radius { get { return totemData.Radius; } }
    
    [SerializeField] TotemData totemData;
    private List<IUnit> enemiesInRange;
    public event EventHandler TargetDisabled;
    private const float buffDelay = 0.33f;


    private void Start() {
        totemData.SetEffectGroup(effectParser.GetEffectGroup(totemData.effectName));
        StartCoroutine(ApplyEffects());
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
