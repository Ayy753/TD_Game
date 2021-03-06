namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using System.Collections.Generic;
    using UnityEngine;

    public class ParticlePool : MonoBehaviour {
        private Dictionary<string, List<ParticleEffect>> particleNameToParticleEFfectList;
        private Dictionary<string, GameObject> particleNameToPrefab;

        private void OnEnable() {
            EffectGroup.OnEffectUsed += EffectGroup_OnEffectUsed;

            InitializeParticleDictionaies();
        }

        private void OnDisable() {
            EffectGroup.OnEffectUsed -= EffectGroup_OnEffectUsed;
        }

        private void EffectGroup_OnEffectUsed(object sender, OnEffectUsedEventArgs e) {
            TryToSpawnParticleEffectAtPosition(e.ParticleName, e.Radius, e.Position);
        }

        private void InitializeParticleDictionaies() {
            GameObject[] particlePrefabs = Resources.LoadAll<GameObject>("Prefabs/Particles");

            particleNameToParticleEFfectList = new Dictionary<string, List<ParticleEffect>>();
            particleNameToPrefab = new Dictionary<string, GameObject>();

            foreach (GameObject prefab in particlePrefabs) {
                particleNameToParticleEFfectList.Add(prefab.name, new List<ParticleEffect>());
                particleNameToPrefab.Add(prefab.name, prefab);
            }
        }

        public void TryToSpawnParticleEffectAtPosition(string particleType, float effectRadius, Vector3 position) {
            try {
                ParticleEffect particleEffect = GetAvailableParticle(particleType);
                if (particleEffect == null) {
                    particleEffect = CreateNewParticleEffectAndAppendToList(particleType);
                }
                particleEffect.Activate(position, effectRadius);
            }
            catch (System.Exception e) {
                Debug.LogError($"{e.Message} when trying to spawn particle type: {particleType}");
            }
        }

        private ParticleEffect GetAvailableParticle(string particleName) {
            foreach (ParticleEffect particleEffect in particleNameToParticleEFfectList[particleName]) {
                if (particleEffect.IsAvailable()) {
                    return particleEffect;
                }
            }
            return null;
        }

        private ParticleEffect CreateNewParticleEffectAndAppendToList(string particleName) {
            GameObject newparticles = GameObject.Instantiate(particleNameToPrefab[particleName]);
            ParticleEffect particleEffect = newparticles.GetComponent<ParticleEffect>();
            particleNameToParticleEFfectList[particleName].Add(particleEffect);
            return particleEffect;
        }
    }
}
