using UnityEngine;

public class ProjectileData : ScriptableObject{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public string Description { get; private set; }

    [SerializeReference]
    public IEffect[] effects;

    public void Init(string name, string description, IEffect[] effect) {
        Name = name;
        Description = description;
        this.effects = effect;
    }
}