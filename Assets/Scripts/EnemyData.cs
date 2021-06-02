using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
public class EnemyData : CharacterData
{
    [field: SerializeField] public int BaseValue { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Type MyType { get; private set; }

    public enum Type {
        Fast,
        Normal,
        Strong
    }
}
