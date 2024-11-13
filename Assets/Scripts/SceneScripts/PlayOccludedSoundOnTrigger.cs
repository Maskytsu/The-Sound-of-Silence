using FMODUnity;
using UnityEngine;

public class PlayOccludedSoundOnTrigger : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private PlayerTrigger _soundTrigger;
    [SerializeField] private Transform _soundPoint;
    [Header("Parameters")]
    [SerializeField] private EventReference _eventRef;

    private void Start()
    {
        _soundTrigger.OnPlayerTriggerEnter += PlaySound;
    }

    private void PlaySound()
    {
        AudioManager.Instance.PlayOneShotOccluded(_eventRef, _soundPoint);
    }
}
