using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class OccludedAudioEmitter : MonoBehaviour
{
    [HideInInspector] public EventInstance eventInstance;

    [SerializeField] private EventReference _eventReference;
    [Space]
    [SerializeField] private float _audioOcclusionWidening = 1f;
    [SerializeField] private float _playerOcclusionWidening = 1f;
    [SerializeField] private bool _shouldRealeseOnStart = true;
    [SerializeField] private float _audioVolume = 1f;

    private AudioManager AudioManager => AudioManager.Instance;

    private void Start()
    {
        PlaySound();
    }

    private void OnDisable()
    {
        StopSound();
    }

    private void OnDestroy()
    {
        StopSound();
    }

    private void PlaySound()
    {
        eventInstance = AudioManager.CreateOccludedInstance(
            _eventReference, transform, _audioOcclusionWidening, _playerOcclusionWidening);

        eventInstance.setVolume(_audioVolume);

        eventInstance.start();
        if (_shouldRealeseOnStart) eventInstance.release();
    }

    private void StopSound()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
