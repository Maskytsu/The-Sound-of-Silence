using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSound : MonoBehaviour
{
    public EventInstance AmbienceEventInstance { get; private set; }
    public EventInstance MusicEventInstance { get; private set; }

    [SerializeField] private EventReference MusicEventRef;
    [SerializeField] private EventReference AmbienceEventRef;

    [SerializeField] private bool _playMusic = true;
    [SerializeField] private bool _playAmbience = true;

    private EventInstance _musicEventInstance;
    private EventInstance _ambienceEventInstance;

    public static BackgroundSound Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one BackgroundSound in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        if (_playMusic)
        {
            _musicEventInstance = RuntimeManager.CreateInstance(MusicEventRef);
            _musicEventInstance.start();
            _musicEventInstance.release();
        }

        if (_playAmbience)
        {
            _ambienceEventInstance = RuntimeManager.CreateInstance(AmbienceEventRef);
            _ambienceEventInstance.start();
            _ambienceEventInstance.release();
        }
    }
}