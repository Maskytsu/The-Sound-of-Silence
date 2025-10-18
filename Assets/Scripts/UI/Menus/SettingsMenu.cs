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
    [SerializeField] private float _maxBrightnessValue = 2;

    private void Awake()
    {
        _volumeSlider.value = (int) Settings.Instance.Volume * 100;
        DisplaySliderValueToTMP(_volumeSlider, _volumeTMP);

        _brightnessSlider.maxValue = _maxBrightnessValue * 100.0f;
        _brightnessSlider.value = (int) Settings.Instance.Brightness * 100;
        DisplaySliderValueToTMP(_brightnessSlider, _brightnessTMP);
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

    private void DisplaySliderValueToTMP(Slider slider, TextMeshProUGUI TMP)
    {
        float value = (int) ((slider.value / slider.maxValue) * 100.0f);
        TMP.text = value.ToString();
    }
}
