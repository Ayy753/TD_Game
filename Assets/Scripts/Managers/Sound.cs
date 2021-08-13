using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound {
    [SerializeField] private List<AudioClip> soundVarients;
    public SoundManager.SoundType soundType;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(.3f, 3f)]
    public float pitch = .3f;

    public AudioClip GetRandomAudioClip() {
        return soundVarients[Random.Range(0, soundVarients.Count)];
    }
}
