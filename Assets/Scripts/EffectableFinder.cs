namespace DefaultNamespace.EffectSystem {
    using System.Collections.Generic;
    using UnityEngine;

    public class EffectableFinder : MonoBehaviour {
        public List<IEffectable> GetEffectableObjectsInRange(Vector3 center, float range) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(center, range);
            List<IEffectable> effectableObjectsInRange = new List<IEffectable>();

            foreach (var collider in colliders) {
                IEffectable effectable = collider.GetComponent<IEffectable>();
                if (effectable != null) {
                    effectableObjectsInRange.Add(effectable);
                }
            }

            return effectableObjectsInRange;
        }
    }
}