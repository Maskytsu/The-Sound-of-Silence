using UnityEngine;

public class NeighbourCar : MonoBehaviour
{
    public Transform DriverPointToLookAt;
    public Transform NeighbourHead;

    private OccludedAudioEmitter _carAmbient;

    private void Start()
    {
        _carAmbient = AudioManager.PlayOneShotOccludedRI(FmodEvents.Instance.SPT_ComingCar, gameObject, true, audioOcclusionWidening: 2f);
    }

    private void OnDestroy()
    {
        _carAmbient.EndAudio();
    }
}