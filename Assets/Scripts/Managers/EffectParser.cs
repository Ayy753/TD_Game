using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EffectParser : MonoBehaviour {
    private const string FilePath = "effects";

    void Start() {
        Projectiles projectiles = null;

        string xmlText = ((TextAsset)Resources.Load(FilePath, typeof(TextAsset))).text;
        var serializer = new XmlSerializer(typeof(Projectiles));
        using (var reader = new StringReader(xmlText)) {
            var projectile = (Projectiles)serializer.Deserialize(reader);
        }

        Debug.Log(projectiles.FrostProjectile.Description);
    }
}

public class Projectiles {
    public FrostProjectile FrostProjectile { get; set; }
    public PoisonProjectile PoisonProjectile { get; set; }
}

public class PoisonProjectile {
    public string Name { get; set; }
    public string Description { get; set; }
    public Effects Effects { get; set; }
}

public class FrostProjectile {
    public string Name { get; set; }
    public string Description { get; set; }
    public Effects Effects { get; set; }
}

public class Effects {
    public Buff Buff { get; set; }
    public List<Damage> Damage { get; set; }
    public DamageOverTime DamageOverTime { get; set; }
}