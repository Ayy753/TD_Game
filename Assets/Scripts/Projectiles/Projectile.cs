using UnityEngine;

public class Projectile : MonoBehaviour {
    public Transform Target { get; private set; }
    public Vector3 LastTargetPosition { get; private set; }
    private EffectGroup effectGroup;

    private bool targetDied;
    private bool dealtDamage;

    private void Update() {
        MoveTowardsTarget();
    }

    public void Initialize(Vector3 startPos, Transform target, EffectGroup effectGroup) {
        transform.position = startPos;
        Target = target;
        targetDied = false;
        dealtDamage = false;
        this.effectGroup = effectGroup;
    }

    protected void MoveTowardsTarget() {
        if (Target.gameObject.activeInHierarchy == false) {
            //  Make sure projectile doesn't start going after an enemy that respawned after dying
            targetDied = true;
        }

        if (targetDied != true) {
            LastTargetPosition = Target.position;
            transform.position = Vector3.MoveTowards(transform.position, Target.position, 5f * Time.deltaTime);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, LastTargetPosition, 5f * Time.deltaTime);
            if (transform.position == LastTargetPosition)
                gameObject.SetActive(false);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        IUnit unit = collision.GetComponentInChildren<IUnit>();

        //  If it hit a unit, and hasn't damaged anything yet (to prevent it from damaging multiple units at the same time)
        if (unit != null && dealtDamage == false) {

            //  If the unit is the target
            if (unit.GetTransform() == Target) {
                dealtDamage = true;
                ApplyEffectGroup(unit);
                gameObject.SetActive(false);
            }
            //  If target is already dead, it can damage another unit
            else if (targetDied == true) {
                ApplyEffectGroup(unit);
                dealtDamage = true;
                gameObject.SetActive(false);
            }
        }
    }

    private void ApplyEffectGroup(IUnit unit) {
        unit.GetStatus().ApplyEffectGroup(effectGroup);
    }
}