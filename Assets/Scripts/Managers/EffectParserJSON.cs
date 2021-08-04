using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;

public class EffectParserJSON : MonoBehaviour {
    private const string FilePath = "effects";
    private List<EffectGroup> effectGroups = new List<EffectGroup>();

    enum EffectType {
        Buff, Damage, DOT, StatMod, Debuff, Heal
    }

    void Start() {
        EffectGroups();
    }

    private class Root {
        [JsonProperty("effectGroup")]
        public ParsedEffectGroup[] effectGroups { get; set; }
    }

    private class ParsedEffectGroup {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("target type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EffectGroup.TargetType TargetType { get; set; }

        [JsonProperty("radius", NullValueHandling = NullValueHandling.Ignore)]
        public float Radius { get; set; } = 0;

        [JsonProperty("effects")]
        public ParsedEffect[] Effects { get; set; }
    }

    private class ParsedEffect {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EffectType EffectType { get; set; }

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
    /// Loads effect groups from json file and creates EffectGroup ScriptableObjects
    /// </summary>
    private void EffectGroups() {
        string jsonText = ((TextAsset)Resources.Load(FilePath, typeof(TextAsset))).text;
        Root root = JsonConvert.DeserializeObject<Root>(jsonText);

        foreach (ParsedEffectGroup parsedEffectGroups in root.effectGroups) {
            CreateEffectGroup(parsedEffectGroups);
        }
    }

    /// <summary>
    /// Converts parsed json data into effect group object
    /// </summary>
    /// <param name="parsedEffectGroup">Parsed effect groups</param>
    private void CreateEffectGroup(ParsedEffectGroup parsedEffectGroup) {
        int effectsLen = parsedEffectGroup.Effects.Length;
        IEffect[] effects = new IEffect[effectsLen];

        for (int i = 0; i < effectsLen; i++) {
            ParsedEffect currentEffect = parsedEffectGroup.Effects[i];
            switch (currentEffect.EffectType) {
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
                case EffectType.Heal:
                    effects[i] = new Heal(currentEffect.Potency);
                    break;
                default:
                    throw new System.Exception("The effect type " + currentEffect.EffectType + " is not valid");
            }
        }
        EffectGroup effectGroup = ScriptableObject.CreateInstance("EffectGroup") as EffectGroup;
        effectGroup.Init(parsedEffectGroup.Name, parsedEffectGroup.Description, effects, parsedEffectGroup.TargetType, parsedEffectGroup.Radius);
        effectGroups.Add(effectGroup);
    }

    public EffectGroup GetEffectGroup(string name) {
        foreach (EffectGroup effectGroup in effectGroups) {
            if (effectGroup.Name == name) {
                return effectGroup;
            }
        }
        throw new System.Exception("The effectGroup named " + name + " does not exist");
    }
}