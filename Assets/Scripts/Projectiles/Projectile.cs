namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using UnityEngine;

    public class Projectile : MonoBehaviour {
        private Transform target;
        private Vector3 lastTargetPosition;
        private EffectGroup effectGroup;
        private bool targetDestroyed;
        private bool effectAlreadyUsed;
        private const float PROJECTILE_SPEED = 5F;

        private void Update() {
            MoveTowardsDestination();
        }

        public void Initialize(Vector3 startPos, IEffectable target, EffectGroup effectGroup) {
            transform.position = startPos;
            this.target = target.GetTransform();
            targetDestroyed = false;
            effectAlreadyUsed = false;
            this.effectGroup = effectGroup;

            Enemy.OnEnemyDied += Enemy_OnEnemyDied;
        }

        private void OnDisable() {
            Enemy.OnEnemyDied -= Enemy_OnEnemyDied;
        }

        private void Enemy_OnEnemyDied(Enemy enemy) {
            if (enemy.transform == target) {
                targetDestroyed = true;
            }
        }

        private void MoveTowardsDestination() {
            if (!targetDestroyed) {
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
            if (transform.position == lastTargetPosition) {

                if (effectGroup.Type == TargetType.Area) {
                    ApplyEffectToArea(transform.position);
                }
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            IEffectable effectable = collision.GetComponentInChildren<IEffectable>();

            //  Prevent effect from being triggered multiple times at once and ignore collisions with other objects if the current target is still active
            if ((effectable != null && !effectAlreadyUsed) && (effectable.GetTransform() == target || targetDestroyed)) {
                effectAlreadyUsed = true;

                if (effectGroup.Type == TargetType.Area) {
                    ApplyEffectToArea(target.position);
                }
                else {
                    ApplyEffectToIndividual(effectable);
                }

                gameObject.SetActive(false);
            }
        }

        private void ApplyEffectToIndividual(IEffectable effectable) {
            effectGroup.EffectTarget(effectable, transform.position);
        }

        private void ApplyEffectToArea(Vector3 center) {
            effectGroup.EffectArea(center);
        }
    }
}
