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
    [SerializeField] private float _maxBrightnessValue = 2f;

    private void Awake()
    {
        _volumeSlider.value = Settings.Instance.Volume;
        DisplaySliderValueToTMP(_volumeSlider, _volumeTMP);

        _brightnessSlider.maxValue = _maxBrightnessValue;
        _brightnessSlider.value = Settings.Instance.Brightness;
        DisplaySliderValueToTMP(_brightnessSlider, _brightnessTMP);
    }

    public void SetVolume()
    {
        Settings.Instance.UpdateVolume(_volumeSlider.value);
        DisplaySliderValueToTMP(_volumeSlider, _volumeTMP);
    }

    public void SetBrightness()
    {
        Settings.Instance.UpdateBrightness(_brightnessSlider.value);
        DisplaySliderValueToTMP(_brightnessSlider, _brightnessTMP);
    }

    private void DisplaySliderValueToTMP(Slider slider, TextMeshProUGUI TMP)
    {
        float value = slider.value * (100 / slider.maxValue);
        TMP.text = value.ToString().Split(",")[0];
    }
}
