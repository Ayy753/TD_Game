namespace DefaultNamespace.IO {

    using DefaultNamespace.GUI;
    using System;
    using UnityEngine;
    using Zenject;

    public class SettingsEventArgs : EventArgs {
        public int TargetFps { get; private set; }
        public float Volume { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int RefreshRate { get; private set; }
        public bool Fullscreen { get; private set; }

        public SettingsEventArgs(int targetFps, float volume, int screenWidth, int screenHeight, int refreshRate, bool fullscreen) {
            TargetFps = targetFps;
            Volume = volume;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            RefreshRate = refreshRate;
            Fullscreen = fullscreen;
        }
    }

    public class Settings : IInitializable, IDisposable {

        const int DEFAULT_FPS = 60;
        const float DEFAULT_VOLUME = 100f;

        private bool fullscreenEnabled;

        public static event EventHandler<SettingsEventArgs> OnSettingsLoaded;

        public void Initialize() {
            SettingsPanel.OnTargetFpsChanged += SettingsPanel_OnTargetFpsChanged;
            SettingsPanel.OnVolumeChanged += SettingsPanel_OnVolumeChanged;
            SettingsPanel.OnResolutionChanged += SettingsPanel_OnResolutionChanged;
            SettingsPanel.OnFullscreenChanged += SettingsPanel_OnFullscreenChanged;
            
            Resolution currentResolution = Screen.currentResolution;

            int targetFps = PlayerPrefs.GetInt("FPS", DEFAULT_FPS);
            float volume = PlayerPrefs.GetFloat("Volume", DEFAULT_VOLUME);
            int screenWidth = PlayerPrefs.GetInt("ScreenWidth", currentResolution.width);
            int screenHeight = PlayerPrefs.GetInt("ScreenHeight", currentResolution.height);
            int refreshRate = PlayerPrefs.GetInt("RefreshRate", currentResolution.refreshRate);
            
            int fullscreenInt = PlayerPrefs.GetInt("Fullscreen", 1);
            if (fullscreenInt == 1) 
                fullscreenEnabled = true;
            else 
                fullscreenEnabled = false;

            OnSettingsLoaded?.Invoke(null, new SettingsEventArgs(targetFps, volume, screenWidth, screenHeight, refreshRate, fullscreenEnabled));
        }

        public void Dispose() {
            SettingsPanel.OnTargetFpsChanged -= SettingsPanel_OnTargetFpsChanged;
            SettingsPanel.OnVolumeChanged -= SettingsPanel_OnVolumeChanged;
            SettingsPanel.OnResolutionChanged -= SettingsPanel_OnResolutionChanged;
            SettingsPanel.OnFullscreenChanged -= SettingsPanel_OnFullscreenChanged;
        }

        private void SettingsPanel_OnVolumeChanged(object sender, VolumeChangedEventArgs e) {
            PlayerPrefs.SetFloat("Volume", e.NormalizedVolume * 100);
        }

        private void SettingsPanel_OnFullscreenChanged(object sender, FullscreenToggleChangedEventArgs e) {
            fullscreenEnabled = e.Fullscreen;
            Screen.fullScreen = fullscreenEnabled;

            if (e.Fullscreen) {
                PlayerPrefs.SetInt("Fullscreen", 1);
            }
            else {
                PlayerPrefs.SetInt("Fullscreen", 0);
            }
        }

        private void SettingsPanel_OnResolutionChanged(object sender, ScreenSettingsChangedEventArgs e) {
            Screen.SetResolution(e.Width, e.Height, fullscreenEnabled);

            PlayerPrefs.SetInt("ScreenWidth", e.Width);
            PlayerPrefs.SetInt("ScreenHeight", e.Height);
            PlayerPrefs.SetInt("RefreshRate", e.RefreshRate);
        }

        private void SettingsPanel_OnTargetFpsChanged(object sender, TargetFpsChangedEventArgs e) {
            PlayerPrefs.SetInt("FPS", e.TargetFps);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = e.TargetFps;
        }
    }
}