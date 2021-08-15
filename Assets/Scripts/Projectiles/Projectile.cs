using UnityEngine;

public class Projectile : MonoBehaviour {
    private Transform target;
    private Vector3 lastTargetPosition;
    private EffectGroup effectGroup;
    private bool targetDestroyed;
    private bool dealtDamage;
    private const float PROJECTILE_SPEED = 5F;

    private void Update() {
        MoveTowardsDestination();
    }

    public void Initialize(Vector3 startPos, Transform target, EffectGroup effectGroup) {
        transform.position = startPos;
        this.target = target;
        targetDestroyed = false;
        dealtDamage = false;
        this.effectGroup = effectGroup;
    }

    private void MoveTowardsDestination() {
        if (target.gameObject.activeInHierarchy == false) {
            targetDestroyed = true;
        }

        if (targetDestroyed != true) {
            PursueTargetAndRecordLastPosition();
        }
        else {
            MoveToLastTargetPosition();
        }
    }

    private void PursueTargetAndRecordLastPosition() {
        transform.position = Vector3.MoveTowards(transform.position, target.position, PROJECTILE_SPEED * Time.deltaTime);
        lastTargetPosition = target.position;
    }

    private void MoveToLastTargetPosition() {
        transform.position = Vector3.MoveTowards(transform.position, lastTargetPosition, PROJECTILE_SPEED * Time.deltaTime);
        if (transform.position == lastTargetPosition)
            ApplyEffectToArea(transform.position);
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        IEffectable effectable = collision.GetComponentInChildren<IEffectable>();

        //  If it hit a unit, and hasn't damaged anything yet (to prevent it from damaging multiple units at the same time)
        if (effectable != null && dealtDamage == false) {
            //  If the unit is the target, or the target died and something else got hit
            if (effectable.GetTransform() == target || targetDestroyed == true) {
                dealtDamage = true;
                ApplyEffectToIndividual(effectable);
                gameObject.SetActive(false);
            }
        }
    }

    private void ApplyEffectToIndividual(IEffectable effectable) {
        if (effectGroup.Type == EffectGroup.TargetType.Individual) {
            effectGroup.EffectTarget(effectable, transform.position);
        }
        else {
            effectGroup.EffectArea(transform.position);
        }
    }

    private void ApplyEffectToArea(Vector3 center) {
        effectGroup.EffectArea(center);
    }
}
