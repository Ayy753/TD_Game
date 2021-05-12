using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField]
    public float BaseHealth { get; private set; }
    [field: SerializeField]
    public int Value { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public float Speed { get; private set; }
    [field: SerializeField]
    public string Description { get; private set; }
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
