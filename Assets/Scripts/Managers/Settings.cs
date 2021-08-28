namespace DefaultNamespace.IO {

    using DefaultNamespace.GUI;
    using System;
    using UnityEngine;
    using Zenject;

    public class SettingsEventArgs : EventArgs {
        public int TargetFps { get; private set; }
        public float Volume { get; private set; }

        public SettingsEventArgs(int targetFps, float volume) {
            TargetFps = targetFps;
            Volume = volume;
        }
    }

    public class Settings : IInitializable, IDisposable {

        public static event EventHandler<SettingsEventArgs> OnSettingsLoaded;

        public void Initialize() {
            SettingsPanel.OnTargetFpsChanged += SettingsPanel_OnTargetFpsChanged;
            SettingsPanel.OnVolumeChanged += SettingsPanel_OnVolumeChanged;

            int targetFps = PlayerPrefs.GetInt("FPS");
            float volume = PlayerPrefs.GetFloat("Volume");

            OnSettingsLoaded?.Invoke(null, new SettingsEventArgs(targetFps, volume));
        }

        public void Dispose() {
            SettingsPanel.OnTargetFpsChanged -= SettingsPanel_OnTargetFpsChanged;
            SettingsPanel.OnVolumeChanged -= SettingsPanel_OnVolumeChanged;
        }

        private void SettingsPanel_OnVolumeChanged(object sender, VolumeChangedEventArgs e) {
            PlayerPrefs.SetFloat("Volume", e.NormalizedVolume * 100);
        }

        private void SettingsPanel_OnTargetFpsChanged(object sender, TargetFpsChangedEventArgs e) {
            PlayerPrefs.SetInt("FPS", e.TargetFps);
        }
    }
}