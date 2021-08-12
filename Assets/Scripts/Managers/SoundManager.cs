using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public Sound[] sounds;
    public enum SoundType {
        arrowHitFlesh,
        arrowHitDirt,
        arrowHitStone,
        arrowFire,
        bluntHit,
        explosionTiny,
        arrowRelease,
        gainGold
    }

    public void OnEnable() {
        Debug.Log("initializing soundmanager");
        InitializeSounds();

        StartCoroutine("Test");
    }

    private IEnumerator Test() {
        while (true) {
            PlaySound(SoundType.explosionTiny);
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void InitializeSounds() {
        foreach (Sound sound in sounds) {
            InitializeSoundVariations(sound);
        }
    }

    private void InitializeSoundVariations(Sound sound) {
        float volume;
        float pitch;
        AudioSource audioSource;

        foreach (AudioClip audioClip in sound.GetSoundVariations()) {
            volume = sound.volume;
            pitch = sound.pitch;
            audioSource = CreateAudioSourceComponent(audioClip, volume, pitch);

            sound.AddAudioSource(audioSource);
        }
    }

    private AudioSource CreateAudioSourceComponent(AudioClip audioClip, float volume, float pitch) {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.clip = audioClip;

        return audioSource;
    }

    /// <summary>
    /// Plays a random varient of a specified sound type
    /// </summary>
    /// <param name="type"></param>
    public void PlaySound(SoundType type) {
        foreach (Sound sound in sounds) {
            if (sound.soundType == type) {
                sound.GetRandomSoundVariation().Play();
            }
        }
    }
}
