using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;

    private Bus masterBus;
    private Bus SFXBus;
    private Bus musicBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    public static AudioManager instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");
    }

    private void Start()
    {
        //InitializeAmbience(FMODEvents.instance.wind);
        //InitializeMusic(FMODEvents.instance.backgroundMusic1);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        SFXBus.setVolume(SFXVolume);
        musicBus.setVolume(musicVolume);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
        ambienceEventInstance.release();
    }
    public void SetAmbienceParameter(float windIntensityValue)
    {
        //global fmod parameter
        RuntimeManager.StudioSystem.setParameterByName("wind_intensity", windIntensityValue);
    }
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
        musicEventInstance.release();
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
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public EventInstance CreateEventInstance(EventReference eventReference, Transform parent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, parent);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, StudioEventEmitter emitter)
    {
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void PauseAllSoundsAndFadeOutMusic()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.setPaused(true);
        }

        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.EventInstance.setPaused(true);
        }
    }

    private void UnPauseAllSoundsAndFadeInMusic()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.setPaused(false);
        }
        
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.EventInstance.setPaused(false);
        }
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
