using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public enum MusicArea
{
    CLOSE_AREA = 0,
    FAR_AREA = 1
}

public class AreaChangeTrigger : MonoBehaviour
{
    [Header("Parameter Change")]
    [SerializeField] private float _windIntensityValue;
    [SerializeField] private MusicArea _area;
    [SerializeField] private LayerMask _playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            SetMusicParameter(_area);
            SetAmbienceParameter(_windIntensityValue);
        }
    }

    private void SetMusicParameter(MusicArea area)
    {
        //global fmod parameter
        RuntimeManager.StudioSystem.setParameterByName("Area", (float)area);
    }

    private void SetAmbienceParameter(float windIntensityValue)
    {
        //global fmod parameter
        RuntimeManager.StudioSystem.setParameterByName("WindIntensity", windIntensityValue);
    }
}