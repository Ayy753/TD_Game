using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public SoundManager.soundType soundType;
    public List<AudioClip> soundVarients;
    [HideInInspector]
    public List<AudioSource> audioSources;
    [Range(0f, 1f)]
    public float volume = 0.2f;
    [Range(.3f, 3f)]
    public float pitch = .3f;
    public bool loop = false;
}
