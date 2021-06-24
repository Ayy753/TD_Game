using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Converters;

public class EffectParserJSON : MonoBehaviour {
    private const string FilePath = "effects";

    enum EffectType {
        Buff, Damage, DOT, StatMod
    }

    void Start() {
        string jsonText = ((TextAsset)Resources.Load(FilePath, typeof(TextAsset))).text;
        Root root = JsonConvert.DeserializeObject<Root>(jsonText);

        foreach (ParsedProjectile projectile in root.Projectiles) {
            Debug.Log(string.Format("Name: {0}, Description: {1}", projectile.Name, projectile.Description));
            foreach (ParsedEffect effect in projectile.Effects) {
                string output = string.Format("Type: {0}, Potency: {1}", effect.Type, effect.Potency);
                switch (effect.Type) {
                    case EffectType.Buff:
                        output += string.Format("StatType:{0}, Duration:{1}", effect.StatType, effect.Duration);
                        break;
                    case EffectType.Damage:
                        output += string.Format("DamageType:{0}", effect.DamageType);
                        break;
                    case EffectType.DOT:
                        output += string.Format("DamageType:{0}, Duration:{1}", effect.DamageType, effect.Duration);
                        break;
                    case EffectType.StatMod:
                        output += string.Format("StatType:{0}", effect.StatType);
                        break;
                    default:
                        break;
                }
                Debug.Log(output);
            }
        }
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
        public float? Duration { get; set; }

        [JsonProperty("damageType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public IDamage.DamageType? DamageType { get; set; }

        [JsonProperty("statType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Status.StatType StatType { get; set; }
    }
}