using FMOD.Studio;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Settings : SingletonMonobehaviour<Settings>
{
    public GlobalVolumeSetting GlobalVolume = new();
    public SubVolumeSetting SFXVolume;
    public SubVolumeSetting AmbientVolume;
    public SubVolumeSetting MusicVolume;

    public BrightnessSetting Brightness = new();
    public CameraSensitivitySetting CameraSensitivity = new();
    public DialogueSpeedSetting DialogueSpeed = new();
    public VSyncSetting VSyncOn = new();
    public BorderlessWindowSetting BorderlessWindow = new();

    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private SaveManager _saveManager;
    [Space]
    [SerializeField] private VolumeProfile _brightnessVolume;

    public VolumeProfile BrightnessVolume => _brightnessVolume;

    private void Start()
    {
        GlobalVolume.ApplySetting();
        SFXVolume.ApplySetting();
        AmbientVolume.ApplySetting();
        MusicVolume.ApplySetting();

        Brightness.ApplySetting();
        CameraSensitivity.ApplySetting();
        DialogueSpeed.ApplySetting();
        VSyncOn.ApplySetting();
        BorderlessWindow.ApplySetting();
    }

    public void InitializeVolumeSettings()
    {
        SFXVolume = new(FmodBuses.SFX);
        AmbientVolume = new(FmodBuses.Ambient);
        MusicVolume = new(FmodBuses.Music);
    }

    //==================================================

    public class GlobalVolumeSetting : FloatSetting
    {
        public override void ApplySetting() => AudioManager.Instance.SetGameVolume(Value);
    }
    //-------------
    public class SubVolumeSetting : FloatSetting
    {
        public Bus volumeBus; 

        public SubVolumeSetting(Bus volumeBus)
        {
            this.volumeBus = volumeBus;
        }

        public override void ApplySetting() => AudioManager.Instance.UpdateSubVolumes();
    }
    //-------------
    public class BrightnessSetting : FloatSetting
    {
        public override float MaxValue { get; } = 2.0f;

        public override void ApplySetting()
        {
            Settings.Instance.BrightnessVolume.TryGet(out ColorAdjustments colorAdjustments);
            colorAdjustments.postExposure.value = Value;
        }
    }
    //-------------
    public class CameraSensitivitySetting : FloatSetting
    {
        public override void ApplySetting() { }
    }
    //-------------
    public class DialogueSpeedSetting : FloatSetting
    {
        public override float MinValue { get; } = 0.25f;
        public override float MaxValue { get; } = 3.0f;

        public override void ApplySetting() { }
    }
    //-------------
    public class VSyncSetting : Setting<bool>
    {
        public override void ApplySetting() => QualitySettings.vSyncCount = Value ? 1 : 0;
    }
    //-------------
    public class BorderlessWindowSetting : Setting<bool>
    {
        public override void ApplySetting()
        {
            Screen.fullScreenMode = Value ? FullScreenMode.FullScreenWindow : FullScreenMode.ExclusiveFullScreen;

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                InputProvider.Instance.UnlockCursor();
            }
        }
    }

    //==================================================

    public abstract class FloatSetting : Setting<float>
    {
        public virtual float MinValue { get; } = 0.0f;
        public virtual float MaxValue { get; } = 1.0f;
    }

    public abstract class Setting<T>
    {
        public T Value { get; private set; }

        public void LoadValue(T value)
        {
            Value = value;
        }

        public void UpdateValue(T value)
        {
            Value = value;
            ApplySetting();
            SaveManager.Instance.SaveSettings();
        }

        public abstract void ApplySetting();
    }
}