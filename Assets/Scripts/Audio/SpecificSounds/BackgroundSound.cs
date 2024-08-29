using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSound : MonoBehaviour
{
    public static BackgroundSound Instance { get; private set; }
    public EventInstance MusicEventInstance { get; private set; }
    public EventInstance AmbienceEventInstance { get; private set; }

    [SerializeField] private EventReference AmbienceEventRef;
    [SerializeField] private EventReference MusicEventRef;

    [SerializeField] private bool _playMusic;
    [SerializeField] private bool _playAmbience;

    private EventInstance _ambienceEventInstance;
    private EventInstance _musicEventInstance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one BackgroundSounds in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        if(_playAmbience)
        {
            _ambienceEventInstance = RuntimeManager.CreateInstance(AmbienceEventRef);
            _ambienceEventInstance.start();
            _ambienceEventInstance.release();
        }

        if(_playMusic)
        {
            _musicEventInstance = RuntimeManager.CreateInstance(MusicEventRef);
            _musicEventInstance.start();
            _musicEventInstance.release();
        }
    }
}
