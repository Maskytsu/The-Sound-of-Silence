using FMOD.Studio;
using UnityEngine;

public class FridgeAmbient : MonoBehaviour
{
    private OccludedAudioEmitter _fridgeAmbient;

    private void Start()
    {
        _fridgeAmbient = AudioManager.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_FridgeAmbient, gameObject, true, audioOcclusionWidening: 0.85f);
    }

    private void OnDestroy()
    {
        _fridgeAmbient.EndAudio();
    }
}