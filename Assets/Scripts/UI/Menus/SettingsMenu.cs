using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Global Volume")]
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _volumeTMP;

    [Header("SFX Volume")]
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI _sfxVolumeTMP;

    [Header("Ambient Volume")]
    [SerializeField] private Slider _ambientVolumeSlider;
    [SerializeField] private TextMeshProUGUI _ambientVolumeTMP;

    [Header("MusicVolume")]
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI _musicVolumeTMP;

    [Header("Brightness")]
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private TextMeshProUGUI _brightnessTMP;
    [SerializeField] private float _maxBrightnessValue = 2.0f;

    [Header("Camera Sensitivity")]
    [SerializeField] private Slider _camSensitivitySlider;
    [SerializeField] private TextMeshProUGUI _camSensitivityTMP;

    [Header("VSync")]
    [SerializeField] private Toggle _vsyncToggle;

    [Header("BorderlessWindow")]
    [SerializeField] private Toggle _borderlessWindowToggle;

    Settings _settings;

    private void Start()
    {
        _settings = Settings.Instance;

        _brightnessSlider.maxValue = _maxBrightnessValue * 100.0f;

        SeutupSettingSlider(_settings.GlobalVolume, _volumeSlider, _volumeTMP);
        SeutupSettingSlider(_settings.SFXVolume, _sfxVolumeSlider, _sfxVolumeTMP);
        SeutupSettingSlider(_settings.AmbientVolume, _ambientVolumeSlider, _ambientVolumeTMP);
        SeutupSettingSlider(_settings.MusicVolume, _musicVolumeSlider, _musicVolumeTMP);

        SeutupSettingSlider(_settings.Brightness, _brightnessSlider, _brightnessTMP);
        SeutupSettingSlider(_settings.CameraSensitivity, _camSensitivitySlider, _camSensitivityTMP);

        SetupSettingToggle(_settings.VSyncOn, _vsyncToggle);
        SetupSettingToggle(_settings.BorderlessWindow, _borderlessWindowToggle);
    }

    private void SeutupSettingSlider(Settings.Setting<float> setting, Slider slider, TextMeshProUGUI tmp)
    {
        slider.value = (int)(setting.Value * 100.0f);
        DisplaySliderValueToTMP(slider, tmp);

        slider.onValueChanged.AddListener(UpdateValue);

        //------------------------------------------

        void UpdateValue(float value)
        {
            setting.UpdateValue(value / 100.0f);
            DisplaySliderValueToTMP(slider, tmp);
        }

        void DisplaySliderValueToTMP(Slider slider, TextMeshProUGUI TMP)
        {
            float value = (int)((slider.value / slider.maxValue) * 100.0f);
            TMP.text = value.ToString();
        }
    }

    private void SetupSettingToggle(Settings.Setting<bool> setting, Toggle toggle)
    {
        toggle.isOn = setting.Value;
        toggle.onValueChanged.AddListener(setting.UpdateValue);
    }
}
