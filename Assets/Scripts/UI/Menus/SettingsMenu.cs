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

    [Header("Dialogue Speed")]
    [SerializeField] private Slider _dialogueSpeedSlider;
    [SerializeField] private TextMeshProUGUI _dialogueSpeedTMP;

    [Header("VSync")]
    [SerializeField] private Toggle _vsyncToggle;

    [Header("BorderlessWindow")]
    [SerializeField] private Toggle _borderlessWindowToggle;

    Settings _settings;

    private void Start()
    {
        //basic slider has toggled WholeNumbers so when needed it must be changed manually on its component

        _settings = Settings.Instance;

        SeutupSettingSlider(_settings.GlobalVolume, _volumeSlider, _volumeTMP);
        SeutupSettingSlider(_settings.SFXVolume, _sfxVolumeSlider, _sfxVolumeTMP);
        SeutupSettingSlider(_settings.AmbientVolume, _ambientVolumeSlider, _ambientVolumeTMP);
        SeutupSettingSlider(_settings.MusicVolume, _musicVolumeSlider, _musicVolumeTMP);

        SeutupSettingSlider(_settings.Brightness, _brightnessSlider, _brightnessTMP, 0.5f);
        SeutupSettingSlider(_settings.CameraSensitivity, _camSensitivitySlider, _camSensitivityTMP);
        SeutupSettingSlider(_settings.DialogueSpeed, _dialogueSpeedSlider, _dialogueSpeedTMP, 1f);

        SetupSettingToggle(_settings.VSyncOn, _vsyncToggle);
        SetupSettingToggle(_settings.BorderlessWindow, _borderlessWindowToggle);
    }

    private void SeutupSettingSlider(Settings.FloatSetting setting, Slider slider, TextMeshProUGUI tmp, float valueMultiplier = 100f)
    {
        slider.minValue = setting.MinValue * valueMultiplier;
        slider.maxValue = setting.MaxValue * valueMultiplier;

        slider.value = setting.Value * valueMultiplier;
        DisplaySliderValueToTMP(slider, tmp);

        slider.onValueChanged.AddListener(UpdateValue);

        //------------------------------------------

        void UpdateValue(float value)
        {
            setting.UpdateValue(value / valueMultiplier);
            DisplaySliderValueToTMP(slider, tmp);
        }

        void DisplaySliderValueToTMP(Slider slider, TextMeshProUGUI TMP)
        {
            float value = slider.value;
            TMP.text = value.ToString("0.##");
        }
    }

    private void SetupSettingToggle(Settings.Setting<bool> setting, Toggle toggle)
    {
        toggle.isOn = setting.Value;
        toggle.onValueChanged.AddListener(setting.UpdateValue);
    }
}
