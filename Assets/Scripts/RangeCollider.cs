using UnityEngine;

/// <summary>
/// Used to detect when an enemy enters or leaves the tower's range
/// </summary>
public class RangeCollider : MonoBehaviour {
    Tower Parent;

    private void Awake() {
    }

    private void Start() {
        Parent = transform.parent.GetComponent<Tower>();
        gameObject.GetComponent<CircleCollider2D>().radius = Parent.TowerData.Range;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null) {
            Parent.EnemyEnteredRange(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null) {
            Parent.EnemyLeftRange(enemy);
        }
    }
}
