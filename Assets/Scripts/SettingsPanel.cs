namespace DefaultNamespace.GUI {

    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using TMPro;
    using DefaultNamespace.IO;

    public class VolumeChangedEventArgs : EventArgs {
        public float NormalizedVolume { get; private set; }

        public VolumeChangedEventArgs(float newVolume) {
            NormalizedVolume = newVolume;
        }
    }

    public class TargetFpsChangedEventArgs : EventArgs {
        public int TargetFps { get; private set; }

        public TargetFpsChangedEventArgs(int targetFps) {
            TargetFps = targetFps;
        }
    }

    public class ScreenSettingsChangedEventArgs : EventArgs {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int RefreshRate { get; private set; }

        public ScreenSettingsChangedEventArgs(int width, int height, int refreshRate) {
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
        }
    }

    public class FullscreenToggleChangedEventArgs : EventArgs {
        public bool Fullscreen { get; private set; }
        
        public FullscreenToggleChangedEventArgs(bool fullscreen) {
            Fullscreen = fullscreen;
        }
    }

    public class SettingsPanel : IInitializable, IDisposable {

        private Slider volumeSlider;
        private Dropdown fpsDropdown, resolutionDropdown;
        private TMP_Text txtVolume;
        private Toggle fullscreenToggle;

        private readonly Resolution[] resolutions = Screen.resolutions;

        public static event EventHandler<VolumeChangedEventArgs> OnVolumeChanged;
        public static event EventHandler<TargetFpsChangedEventArgs> OnTargetFpsChanged;
        public static event EventHandler<ScreenSettingsChangedEventArgs> OnResolutionChanged;
        public static event EventHandler<FullscreenToggleChangedEventArgs> OnFullscreenChanged;

        public void Initialize() {
            Settings.OnSettingsLoaded += Settings_OnSettingsLoaded;

            volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
            fpsDropdown = GameObject.Find("FpsDropDownMenu").GetComponent<Dropdown>();
            txtVolume = GameObject.Find("txtVolume").GetComponent<TMP_Text>();
            resolutionDropdown = GameObject.Find("dropdownResolution").GetComponent<Dropdown>();
            fullscreenToggle = GameObject.Find("toggleFullscreen").GetComponent<Toggle>();

            volumeSlider.onValueChanged.AddListener(delegate { OnVolumeSliderChanged(); });
            fpsDropdown.onValueChanged.AddListener(delegate { OnFpsDropdownChanged(); });
            resolutionDropdown.onValueChanged.AddListener( delegate { OnResolutionDropdownChanged(); });
            fullscreenToggle.onValueChanged.AddListener( delegate { OnFullscreenToggled(); });

            InitializeResolutionMenu();
        }

        public void Dispose() {
            Settings.OnSettingsLoaded -= Settings_OnSettingsLoaded;
        }

        private void InitializeResolutionMenu() {
            foreach (Resolution resolution in resolutions) {
                resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
            }
        }

        private void Settings_OnSettingsLoaded(object sender, SettingsEventArgs e) {
            volumeSlider.value = e.Volume;
            fpsDropdown.value = GetFpsDropdownIndex(e.TargetFps);

            fullscreenToggle.isOn = e.Fullscreen;

            int width = e.ScreenWidth;
            int height = e.ScreenHeight;
            int refreshRate = e.RefreshRate;

            Resolution resolution;
            for (int i = 0; i < resolutions.Length; i++) {
                resolution = resolutions[i];
                if (resolution.width == width && resolution.height == height && resolution.refreshRate == refreshRate) {
                    resolutionDropdown.value = i;
                }
            }

            UpdateVolumeLabel();
        }

        private int GetFpsDropdownIndex(int fpsValue) {
            int result;
            bool success;
            for (int i = 0; i < fpsDropdown.options.Count; i++) {
                success = int.TryParse(fpsDropdown.options[i].text, out result);

                if (success && result == fpsValue) {
                    return i;
                }
            }

            throw new IndexOutOfRangeException($"{fpsValue} is not a valid Target FPS option");
        }

        private void OnVolumeSliderChanged() {
            float volumeNormalized = volumeSlider.value / 100;
            UpdateVolumeLabel();
            OnVolumeChanged?.Invoke(null, new VolumeChangedEventArgs(volumeNormalized));
        }

        private void UpdateVolumeLabel() {
            txtVolume.text = volumeSlider.value.ToString() + "%";
        }

        private void OnFpsDropdownChanged() {
            try {
                int index = fpsDropdown.value;
                int parsedValue = int.Parse(fpsDropdown.options[index].text);
                OnTargetFpsChanged?.Invoke(null, new TargetFpsChangedEventArgs(parsedValue));
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private void OnResolutionDropdownChanged() {
            int index = resolutionDropdown.value;
            Resolution resolution = resolutions[index];
            OnResolutionChanged?.Invoke(null, new ScreenSettingsChangedEventArgs(resolution.width, resolution.height, resolution.refreshRate));
        }

        private void OnFullscreenToggled() {
            OnFullscreenChanged?.Invoke(null, new FullscreenToggleChangedEventArgs(fullscreenToggle.isOn));
        }
    }
}
