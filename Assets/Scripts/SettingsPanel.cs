namespace DefaultNamespace {
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using TMPro;

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

    public class SettingsPanel : IInitializable {
        private Slider volumeSlider;
        private Dropdown fpsDropdown;
        private TMP_Text txtVolume;

        public static event EventHandler<VolumeChangedEventArgs> OnVolumeChanged;
        public static event EventHandler<TargetFpsChangedEventArgs> OnTargetFpsChanged;

        public void Initialize() {
            volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
            fpsDropdown = GameObject.Find("FpsDropDownMenu").GetComponent<Dropdown>();
            txtVolume = GameObject.Find("txtVolume").GetComponent<TMP_Text>();

            volumeSlider.onValueChanged.AddListener(delegate { OnVolumeSliderChanged(); });
            fpsDropdown.onValueChanged.AddListener(delegate { OnFpsDropdownChanged(); });

            UpdateVolumeLabel();
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
