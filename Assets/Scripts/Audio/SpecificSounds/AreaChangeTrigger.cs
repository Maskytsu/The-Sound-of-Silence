using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
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
            SetAmbienceParameter(_windIntensityValue);
            SetMusicParameter(_area);
        }
    }

    private void SetAmbienceParameter(float windIntensityValue)
    {
        //global fmod parameter
        RuntimeManager.StudioSystem.setParameterByName("WindIntensity", windIntensityValue);
    }

    private void SetMusicParameter(MusicArea area)
    {
        //global fmod parameter
        RuntimeManager.StudioSystem.setParameterByName("Area", (float)area);
    }
}
