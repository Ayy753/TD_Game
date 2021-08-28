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

    public class SettingsPanel : IInitializable, IDisposable {

        private Slider volumeSlider;
        private Dropdown fpsDropdown;
        private TMP_Text txtVolume;

        public static event EventHandler<VolumeChangedEventArgs> OnVolumeChanged;
        public static event EventHandler<TargetFpsChangedEventArgs> OnTargetFpsChanged;

        public void Initialize() {
            Settings.OnSettingsLoaded += Settings_OnSettingsLoaded;

            volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
            fpsDropdown = GameObject.Find("FpsDropDownMenu").GetComponent<Dropdown>();
            txtVolume = GameObject.Find("txtVolume").GetComponent<TMP_Text>();

            volumeSlider.onValueChanged.AddListener(delegate { OnVolumeSliderChanged(); });
            fpsDropdown.onValueChanged.AddListener(delegate { OnFpsDropdownChanged(); });
        }

        public void Dispose() {
            Settings.OnSettingsLoaded -= Settings_OnSettingsLoaded;
        }

        private void Settings_OnSettingsLoaded(object sender, SettingsEventArgs e) {
            volumeSlider.value = e.Volume;
            fpsDropdown.value = GetFpsDropdownIndex(e.TargetFps);

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
    }
}
