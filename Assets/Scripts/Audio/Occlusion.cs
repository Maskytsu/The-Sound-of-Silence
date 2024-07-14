using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Occlusion : MonoBehaviour
{
    [HideInInspector] public EventReference selectedAudio;
    [HideInInspector] public EventInstance audioEvent;
    private EventDescription audioDes;
    private StudioListener listener;
    private PLAYBACK_STATE pb;

    [Header("Occlusion Options")]
    [SerializeField] [Range(0f, 10f)] private float soundOcclusionWidening = 1f;
    [SerializeField] [Range(0f, 10f)] private float playerOcclusionWidening = 1f;
    [SerializeField] private LayerMask occlusionLayer;

    private bool audioIsVirtual;
    private float maxDistance;
    private float listenerDistance;
    private float lineCastHitCount = 0f;

    private void Start()
    {
        audioDes = RuntimeManager.GetEventDescription(selectedAudio);
        audioDes.getMinMaxDistance(out float minDistance, out maxDistance);

        listener = FindObjectOfType<StudioListener>();
    }

    private void FixedUpdate()
    {
        audioEvent.isVirtual(out audioIsVirtual);
        audioEvent.getPlaybackState(out pb);
        listenerDistance = Vector3.Distance(transform.position, listener.transform.position);

        if (!audioIsVirtual && pb == PLAYBACK_STATE.PLAYING && listenerDistance <= maxDistance)
            OccludeBetween(transform.position, listener.transform.position);
    }

    private void OccludeBetween(Vector3 sound, Vector3 listener)
    {
        Vector3 soundLeft = CalculatePoint(sound, listener, soundOcclusionWidening, true);
        Vector3 soundRight = CalculatePoint(sound, listener, soundOcclusionWidening, false);

        Vector3 soundAbove = new Vector3(sound.x, sound.y + soundOcclusionWidening, sound.z);
        Vector3 soundBelow = new Vector3(sound.x, sound.y - soundOcclusionWidening, sound.z);

        Vector3 listenerLeft = CalculatePoint(listener, sound, playerOcclusionWidening, true);
        Vector3 listenerRight = CalculatePoint(listener, sound, playerOcclusionWidening, false);

        Vector3 listenerAbove = new Vector3(listener.x, listener.y + playerOcclusionWidening * 0.5f, listener.z);
        Vector3 listenerBelow = new Vector3(listener.x, listener.y - playerOcclusionWidening * 0.5f, listener.z);

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
        CastLine(soundBelow, listenerBelow);

        SetParameter();

        lineCastHitCount = 0f;
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
        RaycastHit[] hit = Physics.RaycastAll(start, (end - start).normalized, Vector3.Distance(start, end), occlusionLayer);
        if (hit.Length == 1)
        {
            lineCastHitCount++;
            Debug.DrawLine(start, end, Color.yellow);
        }
        else if (hit.Length > 1)
        {
            lineCastHitCount++; //to trzeba jakoœ wzmocniæ je¿eli to s¹ te bardziej œrodkowe casty
            Debug.DrawLine(start, end, Color.red);
        }
        else
        {
            Debug.DrawLine(start, end, Color.green);
        }
    }

    private void SetParameter()
    {
        audioEvent.setParameterByName("occlusion", lineCastHitCount / 11);
    }
}
