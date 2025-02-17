using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using NaughtyAttributes;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public bool IsAbleToHear { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private FmodSnapshots _fmodSnapshots;
    [Space]
    [SerializeField] private LayerMask _occlusionLayer;

    private EventInstance _silenceSnapshot;

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
            _silenceSnapshot.start();
        }
        else
        {
            IsAbleToHear = false;
            _silenceSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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

    public EventInstance PlayOneShotOccludedRI(EventReference eventRef, Transform audioParent, float audioOcclusionWidening = 1f, float playerOcclusionWidening = 1f)
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

    public EventInstance PlayOneShotSpatializedRI(EventReference eventRef, Transform audioParent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);
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