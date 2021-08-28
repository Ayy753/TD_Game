namespace DefaultNamespace.SoundSystem {

    using DefaultNamespace.EffectSystem;
    using DefaultNamespace.GUI;
    using UnityEngine;

    public enum SoundType {
        arrowHitFlesh,
        arrowHitDirt,
        arrowHitStone,
        arrowFire,
        bluntHit,
        explosionTiny,
        arrowRelease,
        gainGold,
        buff,
        gunFire
    }

    public class SoundManager : MonoBehaviour {
        [SerializeField] private Sound[] sounds;
        private float globalVolume = 1f;

        public void OnEnable() {
            EffectGroup.OnEffectUsed += EffectGroup_OnEffectUsed;
            Tower.OnProjectileFired += Tower_OnProjectileFired;
            SettingsPanel.OnVolumeChanged += SettingsPanel_OnVolumeChanged;
        }

        private void OnDisable() {
            EffectGroup.OnEffectUsed -= EffectGroup_OnEffectUsed;
            Tower.OnProjectileFired -= Tower_OnProjectileFired;
            SettingsPanel.OnVolumeChanged -= SettingsPanel_OnVolumeChanged;
        }

        private void EffectGroup_OnEffectUsed(object sender, OnEffectUsedEventArgs e) {
            PlaySound(e.SoundType, e.Position);
        }

        private void Tower_OnProjectileFired(object sender, Tower.ProjectileFiredEventArgs e) {
            PlaySound(SoundType.gunFire, e.Position);
        }

        private void SettingsPanel_OnVolumeChanged(object sender, VolumeChangedEventArgs e) {
            SetGlobalVolume(e.NormalizedVolume);
        }

        private void SetGlobalVolume(float normalizedVolume) {
            globalVolume = normalizedVolume;
        }

        public void PlaySound(SoundType type) {
            GameObject soundGameObject = new GameObject("Sound");
            Sound sound = GetSound(type);

            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.volume = CalculateScaledVolume(sound.Volume);
            audioSource.pitch = sound.Pitch;
            audioSource.PlayOneShot(sound.GetRandomAudioClip());

            Destroy(soundGameObject, audioSource.clip.length);
        }

        public void PlaySound(SoundType type, Vector3 position) {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;

            Sound sound = GetSound(type);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.volume = CalculateScaledVolume(sound.Volume);
            audioSource.pitch = sound.Pitch;
            audioSource.clip = sound.GetRandomAudioClip();
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 5;
            audioSource.maxDistance = 60f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.Play();

            Destroy(soundGameObject, audioSource.clip.length);
        }

        private Sound GetSound(SoundType type) {
            foreach (Sound sound in sounds) {
                if (sound.SoundType == type) {
                    return sound;
                }
            }
            Debug.LogWarning($"No sound variants assigned for '{type}' in the SoundManager GameObject via inspector");
            return null;
        }

        private float CalculateScaledVolume(float localVolume) {
            return localVolume * globalVolume;
        }
    }
}
