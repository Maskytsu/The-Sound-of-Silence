using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private LayerMask _occlusionLayer;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene.");
        }
        Instance = this;
    }

    public EventInstance CreateSpatializedInstance(EventReference eventRef, Transform audioParent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);
        return eventInstance;
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

    public EventInstance PlayOneShotSpatialized(EventReference eventRef, Transform audioParent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);
        eventInstance.start();
        eventInstance.release();
        return eventInstance;
    }

    public EventInstance PlayOneShotOccluded(EventReference eventRef, Transform audioParent, float audioOcclusionWidening = 1f, float playerOcclusionWidening = 1f)
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

    public EventInstance PlayOneShotReturnInstance(EventReference eventRef)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        eventInstance.start();
        eventInstance.release();
        return eventInstance;
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

    public void SetBusVolume(Bus bus, float volume)
    {
        volume = Mathf.Clamp01(volume);
        bus.setVolume(volume);
    }

    public void SetInstanceVolume(EventInstance eventInstance, float volume)
    {
        volume = Mathf.Clamp01(volume);
        eventInstance.setVolume(volume);
    }

    /*
    public IEnumerator FadeOutBus(Bus bus, float fadeSpeed)
    {
        bus.getVolume(out float volume);
        while (volume > 0)
        {
            SetBusVolume(bus, volume - fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }

        bus.setPaused(true);
    }

    public IEnumerator FadeInBus(Bus bus, float fadeSpeed)
    {
        bus.setPaused(false);

        bus.getVolume(out float volume);
        while (volume < 1)
        {
            SetBusVolume(bus, volume + fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }
    }
    */
}