using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField]
    public float MaxHealth { get; private set; }
    [field: SerializeField]
    public int Value { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public float Speed { get; private set; }
    [field: SerializeField]
    public string Description { get; set; }
}
