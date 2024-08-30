using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class TestingOcclusion : MonoBehaviour
{
    [SerializeField] private EventReference _eventRef;
    [SerializeField] private EventInstance _audioEvent;

    void Start()
    {
        _eventRef = FmodEvents.Instance.SFX_TestIdleTwo;
        _audioEvent = AudioManager.Instance.CreateOccludedInstance(_eventRef, transform);

        _audioEvent.start();
        _audioEvent.release();
    }

    private void OnEnable()
    {
        _audioEvent.setPaused(false);
    }
    private void OnDisable()
    {
        _audioEvent.setPaused(true);
    }
    private void OnDestroy()
    {
        _audioEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}