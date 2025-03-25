using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    [ReadOnly] public float Volume = 0f;
    [ReadOnly] public float Brightness = 0f;
    [Space]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private SaveManager _saveManager;
    [Space]
    [SerializeField] private VolumeProfile _brightnessVolume;

    private void Awake()
    {
        CreateInstance();
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

        _saveManager.SaveSettings();
    }

    public void UpdateBrightness(float brightness)
    {
        Brightness = brightness;

        _brightnessVolume.TryGet(out ColorAdjustments colorAdjustments);
        colorAdjustments.postExposure.value = Brightness;

        _saveManager.SaveSettings();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one Settings in the scene.");
        }
        Instance = this;
    }
}
