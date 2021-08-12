using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound {
    private List<AudioSource> audioSources = new List<AudioSource>();
    
    [SerializeField] private List<AudioClip> soundVarients;
    public SoundManager.SoundType soundType;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(.3f, 3f)]
    public float pitch = .3f;

    public void AddAudioSource(AudioSource audioSource) {
        audioSources.Add(audioSource);
    }

    public List<AudioClip> GetSoundVariations() {
        return soundVarients;
    }

    public AudioSource GetRandomSoundVariation() {
        return audioSources[Random.Range(0, audioSources.Count)];
    }
}
