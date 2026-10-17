using DG.Tweening;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CameraFlashDialogueEvent), menuName = "ScriptableObjects/DialogueEvents/" + nameof(CameraFlashDialogueEvent))]
public class CameraFlashDialogueEvent : DialogueEvent
{
    [SerializeField] private EventReference _audioEventRef;
    [SerializeField] private float _fullEventDuration;
    [SerializeField] private float _preFlashDuration;
    [SerializeField] private float _flashFadeInDuration;
    [SerializeField] private float _flashDuration;
    [SerializeField] private float _flashFadeOutDuration;
    [SerializeField] private CanvasGroup _cameraFlashPrefab;

    public override float EventDuration => _fullEventDuration;

    public override void HandleEvent()
    {
        var spawnedFlash = Instantiate(_cameraFlashPrefab);
        spawnedFlash.alpha = 0.0f;

        DOTween.Sequence()
            .AppendInterval(_preFlashDuration)
            .AppendCallback(() => RuntimeManager.PlayOneShot(_audioEventRef))
            .Append(spawnedFlash.DOFade(1f, _flashFadeInDuration))
            .AppendInterval(_flashDuration)
            .Append(spawnedFlash.DOFade(0f, _flashFadeOutDuration))
            .SetLink(spawnedFlash.gameObject)
            .OnComplete(() => Destroy(spawnedFlash.gameObject));
    }
}