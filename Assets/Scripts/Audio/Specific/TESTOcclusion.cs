using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class TESTOcclusion : MonoBehaviour
{
    [SerializeField] private EventReference _eventRef;

    private EventInstance _audioEvent;

    void Start()
    {
        _audioEvent = AudioManager.Instance.CreateOccludedInstance(_eventRef, transform);

        _audioEvent.start();
    }

    private void Update()
    {
        PLAYBACK_STATE playbackState;
        _audioEvent.getPlaybackState(out playbackState);

        // If the event has stopped, restart it
        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            _audioEvent.start();
        }
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