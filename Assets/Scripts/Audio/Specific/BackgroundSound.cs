using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class BackgroundSound : MonoBehaviour
{
    public static BackgroundSound Instance { get; private set; }

    [Header("Just for testing now")]
    [SerializeField] private EventReference _musicEventRef;
    [SerializeField] private EventReference _ambienceEventRef;
    [Space]
    [SerializeField] private bool _playMusic = true;
    [SerializeField] private bool _playAmbience = true;

    private EventInstance _musicEventInstance;
    private EventInstance _ambienceEventInstance;

    private void Awake()
    {
        CreateInstance();
    }

    private void Start()
    {
        if (_playMusic)
        {
            _musicEventInstance = RuntimeManager.CreateInstance(_musicEventRef);
            _musicEventInstance.start();
            _musicEventInstance.release();
        }

        if (_playAmbience)
        {
            _ambienceEventInstance = RuntimeManager.CreateInstance(_ambienceEventRef);
            _ambienceEventInstance.start();
            _ambienceEventInstance.release();
        }
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one BackgroundSound in the scene.");
        }
        Instance = this;
    }
}