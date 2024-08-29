using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float _masterVolume = 1;
    [Range(0, 1)]
    public float _SFXVolume = 1;
    [Range(0, 1)]
    public float _musicVolume = 1;

    private Bus _masterBus;
    private Bus _SFXBus;
    private Bus _musicBus;

    private List<EventInstance> _eventInstances;
    private List<StudioEventEmitter> _eventEmitters;

    private EventInstance _ambienceEventInstance;
    private EventInstance _musicEventInstance;
    public static AudioManager _instance {  get; private set; }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        _instance = this;

        _eventInstances = new List<EventInstance>();
        _eventEmitters = new List<StudioEventEmitter>();

        _masterBus = RuntimeManager.GetBus("bus:/");
        _SFXBus = RuntimeManager.GetBus("bus:/SFX");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
    }

    private void Start()
    {
        //InitializeAmbience(FMODEvents.instance.wind);
        //InitializeMusic(FMODEvents.instance.backgroundMusic1);
    }

    private void Update()
    {
        _masterBus.setVolume(_masterVolume);
        _SFXBus.setVolume(_SFXVolume);
        _musicBus.setVolume(_musicVolume);

        //masterBus.setVolume(Settings.volume);
        //SFXBus.setVolume(Settings.volume);
        //musicBus.setVolume(Settings.volume);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        _ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        _ambienceEventInstance.start();
        _ambienceEventInstance.release();
    }
    public void SetAmbienceParameter(float windIntensityValue)
    {
        //global fmod parameter
        RuntimeManager.StudioSystem.setParameterByName("wind_intensity", windIntensityValue);
    }
    private void InitializeMusic(EventReference musicEventReference)
    {
        _musicEventInstance = CreateEventInstance(musicEventReference);
        _musicEventInstance.start();
        _musicEventInstance.release();
    }
    public void SetMusicParameter(MusicArea area)
    {
        //global fmod parameter
        RuntimeManager.StudioSystem.setParameterByName("area", (float)area);
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public EventInstance CreateEventInstance(EventReference eventReference, Transform parent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, parent);
        _eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, StudioEventEmitter emitter)
    {
        emitter.EventReference = eventReference;
        _eventEmitters.Add(emitter);
        return emitter;
    }

    private void PauseAllSoundsAndFadeOutMusic()
    {
        foreach (EventInstance eventInstance in _eventInstances)
        {
            eventInstance.setPaused(true);
        }

        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.EventInstance.setPaused(true);
        }
    }

    private void UnPauseAllSoundsAndFadeInMusic()
    {
        foreach (EventInstance eventInstance in _eventInstances)
        {
            eventInstance.setPaused(false);
        }
        
        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.EventInstance.setPaused(false);
        }
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in _eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
