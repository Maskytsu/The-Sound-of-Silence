using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using NaughtyAttributes;
using UnityEngine.UIElements;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public bool IsAbleToHear { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private FmodSnapshots _fmodSnapshots;
    [Space]
    [SerializeField] private LayerMask _occlusionLayer;

    private EventInstance _silenceSnapshot;

    private Coroutine _fadeInMusicCoroutine;
    private Coroutine _fadeInAmbientCoroutine;
    private Coroutine _fadeOutMusicCoroutine;
    private Coroutine _fadeOutAmbientCoroutine;
    private float _fadeSpeed = 0.05f;

    private void Awake()
    {
        CreateInstance();
        _silenceSnapshot = RuntimeManager.CreateInstance(_fmodSnapshots.Silence);
        ChangeIsAbleToHear(_sceneSetup.IsAbleToHearOnAwake);
    }

    private void OnDestroy()
    {
        _silenceSnapshot.release();
    }

    public void ChangeIsAbleToHear(bool newState)
    {
        if (newState)
        {
            IsAbleToHear = true;
            _silenceSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        else
        {
            IsAbleToHear = false;
            _silenceSnapshot.start();
        }
    }

    public EventInstance CreateOccludedInstance(EventReference eventRef, Transform audioParent, float audioOcclusionWidening = 1f, float playerOcclusionWidening = 1f)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);

        AudioOcclusion audioOcclusion = audioParent.gameObject.AddComponent<AudioOcclusion>();
        audioOcclusion.AudioEvent = eventInstance;
        audioOcclusion.AudioRef = eventRef;
        audioOcclusion.OcclusionLayer = _occlusionLayer;
        audioOcclusion.AudioOcclusionWidening = audioOcclusionWidening;
        audioOcclusion.PlayerOcclusionWidening = playerOcclusionWidening;

        return eventInstance;
    }

    public EventInstance PlayOneShotOccludedRI(EventReference eventRef, Transform audioParent, float audioOcclusionWidening = 1f, float playerOcclusionWidening = 0.75f)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);

        AudioOcclusion audioOcclusion = audioParent.gameObject.AddComponent<AudioOcclusion>();
        audioOcclusion.AudioEvent = eventInstance;
        audioOcclusion.AudioRef = eventRef;
        audioOcclusion.OcclusionLayer = _occlusionLayer;
        audioOcclusion.AudioOcclusionWidening = audioOcclusionWidening;
        audioOcclusion.PlayerOcclusionWidening = playerOcclusionWidening;

        eventInstance.start();
        eventInstance.release();
        return eventInstance;
    }

    public EventInstance PlayOneShotRI(EventReference eventRef)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        eventInstance.start();
        eventInstance.release();
        return eventInstance;
    }

    public float EventLength(EventReference eventRef)
    {
        EventDescription eventDescription = RuntimeManager.GetEventDescription(eventRef);
        eventDescription.getLength(out int lengthMiliseconds);
        float length = (float)lengthMiliseconds / 1000;
        Debug.Log("EventLength: " + length);
        return length;
    }

    public void SetGameVolume(float givenVolume)
    {
        SetBusVolume(FmodBuses.Master, givenVolume);
    }

    public float GetGameVolume()
    {
        FmodBuses.Master.getVolume(out float volume);
        return volume;
    }

    public void SetInstanceVolume(EventInstance eventInstance, float volume)
    {
        volume = Mathf.Clamp01(volume);
        eventInstance.setVolume(volume);
    }

    public void PauseGameplaySounds(bool pauseAmbient, bool pauseMusic)
    {
        FmodBuses.SFX.setPaused(true);

        if (pauseMusic && _fadeInMusicCoroutine != null) StopCoroutine(_fadeInMusicCoroutine);
        if (pauseAmbient && _fadeInAmbientCoroutine != null) StopCoroutine(_fadeInAmbientCoroutine);

        if (pauseMusic) _fadeOutMusicCoroutine = StartCoroutine(FadeOutGameplaySounds(FmodBuses.Music));
        if (pauseAmbient) _fadeOutAmbientCoroutine = StartCoroutine(FadeOutGameplaySounds(FmodBuses.Ambient));
    }

    public void UnpauseGameplaySounds(bool unpauseAmbient, bool unpauseMusic)
    {
        FmodBuses.SFX.setPaused(false);

        if (unpauseMusic && _fadeOutMusicCoroutine != null) StopCoroutine(_fadeOutMusicCoroutine);
        if (unpauseAmbient && _fadeOutAmbientCoroutine != null) StopCoroutine(_fadeOutAmbientCoroutine);

        if (unpauseMusic) _fadeInMusicCoroutine = StartCoroutine(FadeInGameplaySounds(FmodBuses.Music));
        if (unpauseAmbient) _fadeInAmbientCoroutine = StartCoroutine(FadeInGameplaySounds(FmodBuses.Ambient));
    }

    private IEnumerator FadeOutGameplaySounds(Bus bus)
    {
        bus.getVolume(out float volume);
        while (volume > 0)
        {
            SetBusVolume(bus, volume - _fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }

        bus.setPaused(true);
    }

    private IEnumerator FadeInGameplaySounds(Bus bus)
    {
        bus.setPaused(false);

        bus.getVolume(out float volume);
        while (volume < 1)
        {
            SetBusVolume(bus, volume + _fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }
    }

    private void SetBusVolume(Bus bus, float volume)
    {
        volume = Mathf.Clamp01(volume);
        bus.setVolume(volume);
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene.");
        }
        Instance = this;
    }

    //---------------------------------------------------------
    [Button]
    private void SwapIsAbleToHear()
    {
        ChangeIsAbleToHear(!IsAbleToHear);
    }
}