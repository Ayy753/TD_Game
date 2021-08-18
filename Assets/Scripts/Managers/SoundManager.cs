namespace DefaultNamespace.SoundSystem {

    using DefaultNamespace.EffectSystem;
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
            gainGold,
            buff,
            gunFire
        }

        public void OnEnable() {
            EffectGroup.OnEffectUsed += EffectGroup_OnEffectUsed;
            Tower.OnProjectileFired += Tower_OnProjectileFired;
        }

        private void OnDisable() {
            EffectGroup.OnEffectUsed -= EffectGroup_OnEffectUsed;
        }

        private void EffectGroup_OnEffectUsed(object sender, EffectGroup.OnEffectUsedEventArg e) {
            EffectGroup effectGroup = (EffectGroup)sender;
            PlaySound(effectGroup.SoundType, e.position);
        }

        private void Tower_OnProjectileFired(object sender, Tower.ProjectileFiredEventArgs e) {
            PlaySound(SoundType.gunFire, e.Position);
        }

        public void PlaySound(SoundType type) {
            GameObject soundGameObject = new GameObject("Sound");
            Sound sound = GetSound(type);

            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
            audioSource.PlayOneShot(sound.GetRandomAudioClip());

            Destroy(soundGameObject, audioSource.clip.length);
        }

        public void PlaySound(SoundType type, Vector3 position) {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;

            Sound sound = GetSound(type);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
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
                if (sound.soundType == type) {
                    return sound;
                }
            }
            Debug.LogWarning($"No sound variants assigned for '{type}' in the SoundManager GameObject via inspector");
            return null;
        }
    }
}
