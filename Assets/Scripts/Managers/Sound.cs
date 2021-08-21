namespace DefaultNamespace.SoundSystem {

    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Sound {
        [SerializeField] 
        private List<AudioClip> soundVarients;
        
        [SerializeField]
        [Range(0f, 1f)]
        private float volume;
        
        [SerializeField]
        [Range(.3f, 3f)]
        private float pitch;

        [SerializeField]
        private SoundType soundType;

        public SoundType SoundType { get { return soundType; } }
        public float Volume { get { return volume; } } 
        public float Pitch { get { return pitch; } }

        public AudioClip GetRandomAudioClip() {
            return soundVarients[Random.Range(0, soundVarients.Count)];
        }
    }
}
