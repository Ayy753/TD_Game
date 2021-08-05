using UnityEngine;
using System.Collections.Generic;
using System;

public class ParticlePool : MonoBehaviour{
    private Dictionary<ParticleType, List<GameObject>> particleTypeToPrefabList;
    private Dictionary<ParticleType, GameObject> particleTypeToPrefab;

    public enum ParticleType {
        Buff,
        Heal,
        Damage
    }

    private void OnEnable() {
        GameObject[] particlePrefabs = Resources.LoadAll<GameObject>("Prefabs/Particles");

        InitializeParticleDictionaies();
    }

    private void InitializeParticleDictionaies() {
        particleTypeToPrefabList = new Dictionary<ParticleType, List<GameObject>>();
        particleTypeToPrefab = new Dictionary<ParticleType, GameObject>();

        foreach (ParticleType type in Enum.GetValues(typeof(ParticleType))) {
            particleTypeToPrefabList.Add(type, new List<GameObject>());
        }
    }

    public void SpawnParticleEffectAtPosition(ParticleType particleType, Vector3 position) {
        GameObject effect = GetAvailableParticle(particleType);

        if (effect == null) {
            effect = CreateNewParticleEffectAndAppendToList(particleType);
        }

        effect.transform.position = position;
        effect.SetActive(true);
    }

    private GameObject GetAvailableParticle(ParticleType particleType) {
        foreach (GameObject particleGameObject in particleTypeToPrefabList[particleType]) {
            if (particleGameObject.activeInHierarchy == false) {
                return particleGameObject;
            }
        }
        return null;
    }

    private GameObject CreateNewParticleEffectAndAppendToList(ParticleType particleType) {
        GameObject newparticles =  GameObject.Instantiate(particleTypeToPrefab[particleType]);
        particleTypeToPrefabList[particleType].Add(newparticles);
        return newparticles;
    }
}
