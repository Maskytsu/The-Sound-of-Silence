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
    [SerializeField] private string parameterName;
    [SerializeField] private float parameterValue;
    [SerializeField] MusicArea area;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            AudioManager.instance.SetAmbienceParameter(parameterName, parameterValue);
            AudioManager.instance.SetMusicParameter(area);
        }
    }
}
