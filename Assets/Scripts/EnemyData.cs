using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
public class EnemyData : CharacterData {
    [field: SerializeField] public int BaseValue { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public EnemyType Type { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    public enum EnemyType {
        Fast,
        Normal,
        Strong
    }
}
