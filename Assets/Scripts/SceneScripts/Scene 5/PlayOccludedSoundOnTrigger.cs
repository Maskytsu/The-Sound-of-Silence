using FMODUnity;
using UnityEngine;

public class PlayOccludedSoundOnTrigger : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private Trigger _soundTrigger;
    [SerializeField] private Transform _soundPoint;

    private void Start()
    {
        _soundTrigger.OnObjectTriggerEnter += PlaySound;
    }

    private void PlaySound()
    {
        _soundTrigger.gameObject.SetActive(false);
        AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.H_OCC_Creak, _soundPoint);
    }
}