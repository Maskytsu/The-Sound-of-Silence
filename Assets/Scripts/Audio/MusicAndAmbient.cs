using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicAndAmbient : MonoBehaviour
{
    public static MusicAndAmbient Instance { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;

    private EventInstance _backgroundMusicEvent;
    private EventInstance _ambientEvent;

    private void Awake()
    {
        CreateInstance();
        StartMusicAndAmbient();
    }

    private void OnDestroy()
    {
        StopMusicAndAmbient();
    }

    private void StartMusicAndAmbient()
    {
        if (!_sceneSetup.BackgroundMusic.IsNull)
        {
            _backgroundMusicEvent = RuntimeManager.CreateInstance(_sceneSetup.BackgroundMusic);
            if (_sceneSetup.IsBackgroundMusic3D)
            {
                _backgroundMusicEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            }
            _backgroundMusicEvent.start();
            _backgroundMusicEvent.release();
        }

        if (!_sceneSetup.Ambient.IsNull)
        {
            _ambientEvent = RuntimeManager.CreateInstance(_sceneSetup.Ambient);
            if (_sceneSetup.IsAmbient3D)
            {
                _ambientEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            }
            _ambientEvent.start();
            _ambientEvent.release();
        }
    }

    private void StopMusicAndAmbient()
    {
        if (_backgroundMusicEvent.isValid()) _backgroundMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        if (_ambientEvent.isValid()) _ambientEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one MusicAndAmbient in the scene.");
        }
        Instance = this;
    }
}