using FMOD.Studio;
using UnityEngine;

public class FridgeAmbient : MonoBehaviour
{
    private EventInstance _fridgeAmbient;

    private void Start()
    {
        _fridgeAmbient = AudioManager.Instance.PlayOneShotOccludedRI(
            FmodEvents.Instance.OCC_FridgeAmbient, transform);
    }

    private void OnDestroy()
    {
        _fridgeAmbient.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
