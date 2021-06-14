using UnityEngine;

/// <summary>
/// Used to detect when a unit enters or leaves collider's range
/// </summary>
public class RangeCollider : MonoBehaviour {
    IUnitRangeDetection Parent;
    CircleCollider2D theCollider;

    private void Awake() {
        Parent = transform.parent.GetComponent<IUnitRangeDetection>();
        theCollider = gameObject.GetComponent<CircleCollider2D>();
        theCollider.radius = Parent.GetRange();
        Debug.Log(Parent.ToString() + " radius: " + theCollider.radius);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null) {
            Parent.UnitEnteredRange(unit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null) {
            Parent.UnitLeftRange(unit);
        }
    }
}
