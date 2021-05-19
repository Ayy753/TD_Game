using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    [field: SerializeField] public float BaseHealth { get; private set; }
    [field: SerializeField] public float BaseFireResist { get; private set; }
    [field: SerializeField] public float BaseColdResist { get; private set; }
    [field: SerializeField] public float BasePoisonResist { get; private set; }
    [field: SerializeField] public float BaseArmor { get; private set; }
    [field: SerializeField] public float BaseSpeed { get; private set; }
}
