using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioOcclusion : MonoBehaviour
{
    public bool DrawRays = false;
    public EventReference AudioRef;

    [HideInInspector] public EventInstance AudioEvent;
    [HideInInspector] public LayerMask OcclusionLayer;
    [HideInInspector] public float AudioOcclusionWidening = 1f;
    [HideInInspector] public float PlayerOcclusionWidening = 1f;

    private StudioListener _listener;
    private float _maxDistance;
    private float _lineCastHitCount;

    private void Start()
    {
        EventDescription audioDes = RuntimeManager.GetEventDescription(AudioRef);
        audioDes.getMinMaxDistance(out float minDistance, out _maxDistance);

        _listener = FindObjectOfType<StudioListener>();
    }

    private void FixedUpdate()
    {
        AudioEvent.isVirtual(out bool audioIsVirtual);
        if (audioIsVirtual) return;

        AudioEvent.getPlaybackState(out PLAYBACK_STATE pb);
        if (pb != PLAYBACK_STATE.PLAYING) return;

        float listenerDistance = Vector3.Distance(transform.position, _listener.transform.position);
        if (listenerDistance <= _maxDistance) OccludeBetween(transform.position, _listener.transform.position);
    }

    private void OccludeBetween(Vector3 sound, Vector3 listener)
    {
        _lineCastHitCount = 0f;

        Vector3 soundLeft = CalculatePoint(sound, listener, AudioOcclusionWidening, true);
        Vector3 soundRight = CalculatePoint(sound, listener, AudioOcclusionWidening, false);

        Vector3 soundAbove = new Vector3(sound.x, sound.y + AudioOcclusionWidening, sound.z);
        //Vector3 soundBelow = new Vector3(sound.x, sound.y - soundOcclusionWidening, sound.z);

        Vector3 listenerLeft = CalculatePoint(listener, sound, PlayerOcclusionWidening, true);
        Vector3 listenerRight = CalculatePoint(listener, sound, PlayerOcclusionWidening, false);

        Vector3 listenerAbove = new Vector3(listener.x, listener.y + PlayerOcclusionWidening * 0.5f, listener.z);
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
        RaycastHit[] hit = Physics.RaycastAll(start, (end - start).normalized, Vector3.Distance(start, end), OcclusionLayer);
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
        AudioEvent.getParameterByName("Occlusion", out float value);

        if (value > (_lineCastHitCount / 20) + 0.002f) //+ 0.002f is correction for floating point imprecision
        {
            value = value - 0.025f;
            AudioEvent.setParameterByName("Occlusion", (float)value);
        }
        else if (value < (_lineCastHitCount / 20) - 0.002f) //- 0.002f is correction for floating point imprecision
        {
            value = value + 0.025f;
            AudioEvent.setParameterByName("Occlusion", (float)value);
        }
    }
}