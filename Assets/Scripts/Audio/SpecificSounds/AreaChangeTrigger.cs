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
    [SerializeField] private float windIntensityValue;
    [SerializeField] MusicArea area;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            AudioManager.instance.SetAmbienceParameter(windIntensityValue);
            AudioManager.instance.SetMusicParameter(area);
        }
    }
}
