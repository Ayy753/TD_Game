namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class Tower : MonoBehaviour, Itargetable {
        private List<IEffectable> effectableObjectsInRange = new List<IEffectable>();
        private float timeSinceLastShot = float.MaxValue;
        private float cooldown;
        private IEffectable target;
        private Transform turret;
        private EffectableFinder effectableFinder;

        public event EventHandler TargetDisabled;
        public static event EventHandler<ProjectileFiredEventArgs> OnProjectileFired;

        public class ProjectileFiredEventArgs : EventArgs {
            public EffectGroup EffectGroup { get; set; }
            public Vector3 Position { get; set; }
            public IEffectable Target { get; set; }
        }

        [field: SerializeField] public TowerData TowerData { get; private set; }
        public TargetMode CurrentTargetMode { get; private set; }

        public float Radius { get { return TowerData.Range; } }

        public enum TargetMode {
            Closest,
            Furthest,
            Random,
            LowestHealth,
            HighestHealth
        }

        private void Awake() {
            turret = transform.Find("Turret");
            effectableFinder = GameObject.Find("EffectableFinder").GetComponent<EffectableFinder>();
            cooldown = TowerData.EffectGroup.Cooldown;

            StartCoroutine(TurretTracking());
            StartCoroutine(TargetFinder());
        }

        private void OnEnable() {
            Enemy.OnEnemyDied += Enemy_OnEnemyDied;
            Enemy.OnEnemyReachedGate += Enemy_OnEnemyDied;
        }

        private void OnDisable() {
            Enemy.OnEnemyDied -= Enemy_OnEnemyDied;
            Enemy.OnEnemyReachedGate -= Enemy_OnEnemyDied;
        }

        private void Enemy_OnEnemyDied(Enemy enemy) {
            if (enemy == target) {
                target = null;
            }
        }

        private void Update() {
            ShootLogicTick();
        }

        private void ShootLogicTick() {
            timeSinceLastShot += Time.deltaTime;

            if (target != null && timeSinceLastShot >= cooldown) {
                FaceTarget(target.GetTransform());

                OnProjectileFired?.Invoke(null, new ProjectileFiredEventArgs {
                    EffectGroup = TowerData.EffectGroup,
                    Position = transform.position,
                    Target = target
                });

                timeSinceLastShot = 0;
            }
        }

        /// <summary>
        /// Rotates turret to face target
        /// </summary>
        /// <param name="target"></param>
        private void FaceTarget(Transform target) {
            Vector3 vectorDiff = target.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorDiff.y, vectorDiff.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// Rotates the turrent to face current target every 100ms
        /// </summary>
        /// <returns></returns>
        private IEnumerator TurretTracking() {
            while (true) {
                if (target != null) {
                    FaceTarget(target.GetTransform());
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// Periodically searches for a new target
        /// </summary>
        /// <returns></returns>
        private IEnumerator TargetFinder() {
            //  Prevent tower from detecting enemies before it gets moved from origin on instantiation
            yield return new WaitForSeconds(0.33f);
            while (true) {
                effectableObjectsInRange = GetEffectableObjectsInRange(transform.position);
                target = FindTarget();
                yield return new WaitForSeconds(0.33f);
            }
        }

        private List<IEffectable> GetEffectableObjectsInRange(Vector3 position) {
            return effectableFinder.GetEffectableObjectsInRange(position, Radius);
        }

        /// <summary>
        /// Selects the most suitable target based on the selected target mode
        /// </summary>
        /// <returns></returns>
        private IEffectable FindTarget() {
            target = null;

            switch (CurrentTargetMode) {
                case TargetMode.Closest:
                    target = FindClosestEnemy();
                    break;
                case TargetMode.Furthest:
                    target = FindFurthestEnemy();
                    break;
                case TargetMode.HighestHealth:
                    target = FindHighestHealthEnemy();
                    break;
                case TargetMode.LowestHealth:
                    target = FindLowestHealthEnemy();
                    break;
                case TargetMode.Random:
                    target = FindRandomEnemy();
                    break;
                default:
                    throw new Exception("Target mode invalid");
            }
            return target;
        }

        /// <summary>
        /// Selects the enemy closest to tower, within range
        /// </summary>
        /// <returns></returns>
        private IEffectable FindClosestEnemy() {
            IEffectable closestEffectable = null;
            float shortestDistance = float.MaxValue;

            foreach (IEffectable effectable in effectableObjectsInRange) {
                float distance = Distance(transform.position, effectable.GetTransform().position);
                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    closestEffectable = effectable;
                }
            }
            return closestEffectable;
        }

        /// <summary>
        /// Selects the enemy furthest from tower, within range
        /// </summary>
        /// <returns></returns>
        private IEffectable FindFurthestEnemy() {
            IEffectable closestEnemy = null;
            float furthestDistance = float.MinValue;

            foreach (IEffectable effectable in effectableObjectsInRange) {
                float distance = Distance(transform.position, effectable.GetTransform().position);
                if (distance > furthestDistance) {
                    furthestDistance = distance;
                    closestEnemy = effectable;
                }
            }
            return closestEnemy;
        }

        /// <summary>
        /// Selects the enemy with the lowest health in range
        /// </summary>
        /// <returns></returns>
        private IEffectable FindLowestHealthEnemy() {
            IEffectable lowestEnemy = null;
            float lowestHealth = float.MaxValue;

            foreach (IEffectable effectable in effectableObjectsInRange) {
                float health = effectable.GetStatus().Health.Value;
                if (health < lowestHealth) {
                    lowestHealth = health;
                    lowestEnemy = effectable;
                }
            }
            return lowestEnemy;
        }

        /// <summary>
        /// Selects the enemy with the highest health in range
        /// </summary>
        /// <returns></returns>
        private IEffectable FindHighestHealthEnemy() {
            IEffectable highestEnemy = null;
            float highestHealth = float.MinValue;

            foreach (IEffectable effectable in effectableObjectsInRange) {
                float health = effectable.GetStatus().Health.Value;
                if (health > highestHealth) {
                    highestHealth = health;
                    highestEnemy = effectable;
                }
            }
            return highestEnemy;
        }

        /// <summary>
        /// Selects a random enemy within range
        /// </summary>
        /// <returns></returns>
        private IEffectable FindRandomEnemy() {
            if (effectableObjectsInRange.Count > 0) {
                int index = UnityEngine.Random.Range(0, effectableObjectsInRange.Count);
                return effectableObjectsInRange[index];
            }
            return null;
        }

        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        private float Distance(Vector3 start, Vector3 finish) {
            return Mathf.Sqrt(Mathf.Pow(finish.x - start.x, 2f) + Mathf.Pow(finish.y - start.y, 2f));
        }

        public float GetRange() {
            return TowerData.Range;
        }

        public void ChangeTargetMode(TargetMode targetMode) {
            CurrentTargetMode = targetMode;
        }

        /// <summary>
        /// Alerts tower it is being destroyed so it can fire it's targetdisabled event
        /// </summary>
        public void IsBeingDestroyed() {
            if (TargetDisabled != null) {
                TargetDisabled.Invoke(this, EventArgs.Empty);
            }
        }

        public Transform GetTransform() {
            return transform;
        }

        public string GetDescription() {
            return TowerData.Description;
        }

        public string GetName() {
            return TowerData.Name;
        }

        public class Factory : PlaceholderFactory<Tower> { }
    }
}
