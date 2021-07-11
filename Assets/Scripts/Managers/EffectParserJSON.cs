using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EffectParserJSON : MonoBehaviour {
    private const string FilePath = "effects";
    private List<ProjectileData> projectileDatas = new List<ProjectileData>();

    enum EffectType {
        Buff, Damage, DOT, StatMod, Debuff
    }

    void Start() {
        LoadProjectileData();
    }

    private class Root {
        [JsonProperty("projectiles")]
        public ParsedProjectile[] Projectiles { get; set; }
    }

    private class ParsedProjectile {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("effects")]
        public ParsedEffect[] Effects { get; set; }
    }

    private class ParsedEffect {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EffectType Type { get; set; }

        [JsonProperty("potency")]
        public float Potency { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public float Duration { get; set; }

        [JsonProperty("damageType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public IDamage.DamageType DamageType { get; set; }

        [JsonProperty("statType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Status.StatType StatType { get; set; }

        [JsonProperty("resistType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public IDamage.DamageType ResistType { get; set; }
    }

    /// <summary>
    /// Loads projectile data from json file and creates ProjectileData ScriptableObject assets
    /// </summary>
    private void LoadProjectileData() {
        string jsonText = ((TextAsset)Resources.Load(FilePath, typeof(TextAsset))).text;
        Root root = JsonConvert.DeserializeObject<Root>(jsonText);

        foreach (ParsedProjectile parsedProjectile in root.Projectiles) {
            CreateProjectileDataAsset(parsedProjectile);
        }
    }

    /// <summary>
    /// Converts parsed json data into projectile data asset
    /// </summary>
    /// <param name="parsedProjectile">Parsed projectile data</param>
    private void CreateProjectileDataAsset(ParsedProjectile parsedProjectile) {
        int effectsLen = parsedProjectile.Effects.Length;
        IEffect[] effects = new IEffect[effectsLen];

        for (int i = 0; i < effectsLen; i++) {
            ParsedEffect currentEffect = parsedProjectile.Effects[i];
            switch (currentEffect.Type) {
                case EffectType.Buff:
                    effects[i] = new Buff(currentEffect.Potency, currentEffect.Duration, currentEffect.StatType);
                    break;
                case EffectType.Damage:
                    effects[i] = new Damage(currentEffect.Potency, currentEffect.DamageType);
                    break;
                case EffectType.DOT:
                    effects[i] = new DamageOverTime(currentEffect.Potency, currentEffect.Duration, currentEffect.DamageType);
                    break;
                case EffectType.StatMod:
                    effects[i] = new StatMod(currentEffect.Potency, currentEffect.StatType);
                    break;
                case EffectType.Debuff:
                    effects[i] = new Debuff(currentEffect.Potency, currentEffect.Duration, currentEffect.StatType, currentEffect.ResistType);
                    break;
                default:
                    throw new System.Exception("The effect type " + currentEffect.Type + " is not valid");
            }
        }
        ProjectileData projectileData = ScriptableObject.CreateInstance("ProjectileData") as ProjectileData;
        projectileData.Init(parsedProjectile.Name, parsedProjectile.Description, effects);
        projectileDatas.Add(projectileData);
    }

    public ProjectileData GetProjectileData(string name) {
        foreach (ProjectileData projectileData in projectileDatas) {
            if (projectileData.Name == name) {
                return projectileData;
            }
        }
        throw new System.Exception("The projectiledata named " + name + " does not exist");
    }
}