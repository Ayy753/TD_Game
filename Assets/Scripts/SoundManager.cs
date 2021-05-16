using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    public enum soundType
    {
        arrowHitFlesh,
        arrowHitDirt,
        arrowHitStone,
        arrowFire, 
        bluntHit,
        explosionTiny
    }

    public void Awake()
    {
        //  Loop through each sound type
        foreach (Sound sound in sounds)
        {
            sound.audioSources = new List<AudioSource>();

            //  Create and initialize an AudioSource component for each varient of this sound type
            foreach (AudioClip audioClip in sound.soundVarients)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                sound.audioSources.Add(audioSource);
                audioSource.volume = sound.volume;
                audioSource.pitch = sound.pitch;
                audioSource.clip = audioClip;
            }
        }
    }

    /// <summary>
    /// Plays a random varient of a specified sound type
    /// </summary>
    /// <param name="type"></param>
    public void PlaySound(soundType type)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.soundType == type)
            {
                sound.audioSources[Random.Range(0, sound.audioSources.Count)].Play();
            }
        }
    }
}
