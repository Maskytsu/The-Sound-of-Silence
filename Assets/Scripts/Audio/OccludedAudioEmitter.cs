using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class OccludedAudioEmitter : MonoBehaviour
{
    [ShowNativeProperty] public string EventName => _eventRef.IsNull ? " " : _eventRef.Path;
    [ShowNativeProperty] public int EventLeftTime => GetEventLeftTime();
    public EventInstance EventInstance => _eventInstance;

    public bool DrawRays = false;

    private EventReference _eventRef;
    private EventInstance _eventInstance;
    private LayerMask _occlusionLayer;
    private float _audioOcclusionWidening = 1f;
    private float _playerOcclusionWidening = 1f;

    private bool _isLooped;

    private StudioListener _listener;
    private float _maxDistance;
    private float _lineCastHitCount;

    public void Initialize(EventInstance eventInstance, EventReference eventRef, LayerMask occlusionLayer, float audioOcclusionWidening, float playerOcclusionWidening, bool isLooped)
    {
        _eventInstance = eventInstance;
        _eventRef = eventRef;
        _occlusionLayer = occlusionLayer;
        _audioOcclusionWidening = audioOcclusionWidening;
        _playerOcclusionWidening = playerOcclusionWidening;
        _isLooped = isLooped;
    }

    public void EndAudio()
    {
        _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Destroy(this);
    }

    private void Start()
    {
        EventDescription audioDes = RuntimeManager.GetEventDescription(_eventRef);
        audioDes.getMinMaxDistance(out float minDistance, out _maxDistance);

        _listener = FindAnyObjectByType<StudioListener>();
    }

    private void FixedUpdate()
    {
        if (DestroyThisIfNeeded()) return;

        _eventInstance.isVirtual(out bool audioIsVirtual);
        if (audioIsVirtual) return;

        _eventInstance.getPlaybackState(out PLAYBACK_STATE pb);
        if (pb != PLAYBACK_STATE.PLAYING) return;

        float listenerDistance = Vector3.Distance(transform.position, _listener.transform.position);
        if (listenerDistance <= _maxDistance) OccludeBetween(transform.position, _listener.transform.position);
    }

    private void OccludeBetween(Vector3 sound, Vector3 listener)
    {
        _lineCastHitCount = 0f;

        Vector3 soundLeft = CalculatePoint(sound, listener, _audioOcclusionWidening, true);
        Vector3 soundRight = CalculatePoint(sound, listener, _audioOcclusionWidening, false);

        Vector3 soundAbove = new Vector3(sound.x, sound.y + _audioOcclusionWidening, sound.z);
        //Vector3 soundBelow = new Vector3(sound.x, sound.y - soundOcclusionWidening, sound.z);

        Vector3 listenerLeft = CalculatePoint(listener, sound, _playerOcclusionWidening, true);
        Vector3 listenerRight = CalculatePoint(listener, sound, _playerOcclusionWidening, false);

        Vector3 listenerAbove = new Vector3(listener.x, listener.y + _playerOcclusionWidening * 0.5f, listener.z);
        //Vector3 listenerBelow = new Vector3(listener.x, listener.y - playerOcclusionWidening * 0.5f, listener.z);

        CastLine(soundLeft, listenerLeft);
        CastLine(soundLeft, listener);
        CastLine(soundLeft, listenerRight);

        CastLine(sound, listenerLeft);
        CastLine(sound, listener);
        CastLine(sound, listenerRight);

        CastLine(soundRight, listenerLeft);
        CastLine(soundRight, listener);
        CastLine(soundRight, listenerRight);

        CastLine(soundAbove, listenerAbove);
        //CastLine(soundBelow, listenerBelow);

        SetParameter();
    }

    private Vector3 CalculatePoint(Vector3 a, Vector3 b, float m, bool left)
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
        float mn = (m / n);
        if (left)
        {
            x = a.x + (mn * (a.z - b.z));
            z = a.z - (mn * (a.x - b.x));
        }
        else
        {
            x = a.x - (mn * (a.z - b.z));
            z = a.z + (mn * (a.x - b.x));
        }
        return new Vector3(x, a.y, z);
    }

    private void CastLine(Vector3 start, Vector3 end)
    {
        RaycastHit[] hit = Physics.RaycastAll(start, (end - start).normalized, Vector3.Distance(start, end), _occlusionLayer);
        if (hit.Length == 1)
        {
            _lineCastHitCount++;
            if (DrawRays) Debug.DrawLine(start, end, Color.yellow);
        }
        else if (hit.Length > 1)
        {
            _lineCastHitCount += 2;
            if (DrawRays) Debug.DrawLine(start, end, Color.red);
        }
        else
        {
            if (DrawRays) Debug.DrawLine(start, end, Color.green);
        }
    }

    private void SetParameter()
    {
        //max value of occlusion is 1 and we can get it only when all lines are hitting more than 1 walls
        //max occlusion value that we can get with only 1 wall is 0.5f
        _eventInstance.getParameterByName("Occlusion", out float value);

        if (value > (_lineCastHitCount / 20) + 0.002f) //+ 0.002f is correction for floating point imprecision
        {
            value = value - 0.025f;
            _eventInstance.setParameterByName("Occlusion", (float)value);
        }
        else if (value < (_lineCastHitCount / 20) - 0.002f) //- 0.002f is correction for floating point imprecision
        {
            value = value + 0.025f;
            _eventInstance.setParameterByName("Occlusion", (float)value);
        }
    }

    private bool DestroyThisIfNeeded()
    {
        if (!_isLooped && GetEventLeftTime() <= 1)
        {
            Destroy(this);
            return true;
        }

        return false;
    }

    private int GetEventLeftTime()
    {
        RuntimeManager.GetEventDescription(_eventRef).getLength(out int lengthMiliseconds);
        _eventInstance.getTimelinePosition(out int timelinePositionMiliseconds);
        return lengthMiliseconds - timelinePositionMiliseconds;
    }
}