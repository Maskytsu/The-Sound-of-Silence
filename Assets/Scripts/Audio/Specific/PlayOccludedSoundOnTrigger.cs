using FMODUnity;
using UnityEngine;

public class PlayOccludedSoundOnTrigger : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private Trigger _soundTrigger;
    [SerializeField] private Transform _soundPoint;
    [Header("Parameters")]
    [SerializeField] private EventReference _eventRef;

    private void Start()
    {
        _soundTrigger.OnObjectTriggerEnter += PlaySound;
    }

    private void PlaySound()
    {
        _soundTrigger.gameObject.SetActive(false);
        AudioManager.Instance.PlayOneShotOccluded(_eventRef, _soundPoint);
    }
}
