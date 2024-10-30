using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public float Volume = 1f;
    public float Brightness = 0f;

    public static Settings Instance { get; private set; }

    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private VolumeProfile _brightnessVolume;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one Settings in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        _audioManager.SetGameVolume(Volume);

        _brightnessVolume.TryGet(out ColorAdjustments colorAdjustments);
        colorAdjustments.postExposure.value = Brightness;
    }

    public void UpdateVolume(float volume)
    {
        Volume = volume;

        _audioManager.SetGameVolume(Volume);

        SaveManager.Instance.SaveSettings();
    }

    public void UpdateBrightness(float brightness)
    {
        Brightness = brightness;

        _brightnessVolume.TryGet(out ColorAdjustments colorAdjustments);
        colorAdjustments.postExposure.value = Brightness;

        SaveManager.Instance.SaveSettings();
    }
}
