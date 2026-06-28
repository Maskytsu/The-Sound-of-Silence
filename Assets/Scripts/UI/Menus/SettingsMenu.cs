using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _volumeTMP;

    [Header("Brightness")]
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private TextMeshProUGUI _brightnessTMP;
    [SerializeField] private float _maxBrightnessValue = 2.0f;

    [Header("Camera Sensitivity")]
    [SerializeField] private Slider _camSensitivitySlider;
    [SerializeField] private TextMeshProUGUI _camSensitivityTMP;

    private void Start()
    {
        _volumeSlider.value = (int)(Settings.Instance.Volume * 100.0f);
        DisplaySliderValueToTMP(_volumeSlider, _volumeTMP);

        _brightnessSlider.maxValue = _maxBrightnessValue * 100.0f;
        _brightnessSlider.value = (int)(Settings.Instance.Brightness * 100.0f);
        DisplaySliderValueToTMP(_brightnessSlider, _brightnessTMP);

        _camSensitivitySlider.value = (int)(Settings.Instance.CameraSensitivity * 100.0f);
        DisplaySliderValueToTMP(_camSensitivitySlider, _camSensitivityTMP);
    }

    public void SetVolume()
    {
        Settings.Instance.UpdateVolume(_volumeSlider.value / 100.0f);
        DisplaySliderValueToTMP(_volumeSlider, _volumeTMP);
    }

    public void SetBrightness()
    {
        Settings.Instance.UpdateBrightness(_brightnessSlider.value / 100.0f);
        DisplaySliderValueToTMP(_brightnessSlider, _brightnessTMP);
    }

    public void SetCameraSensitivity()
    {
        Settings.Instance.UpdateCameraSensitiviy(_camSensitivitySlider.value / 100.0f);
        DisplaySliderValueToTMP(_camSensitivitySlider, _camSensitivityTMP);
    }

    private void DisplaySliderValueToTMP(Slider slider, TextMeshProUGUI TMP)
    {
        float value = (int) ((slider.value / slider.maxValue) * 100.0f);
        TMP.text = value.ToString();
    }
}
