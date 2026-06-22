using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class Door : Interactable
{
    public bool IsOpened => _isOpened;
    public InteractionHitbox InteractionHitbox => _interactionHitbox;

    [Space]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _openedYRotation;
    [DisableIf(nameof(IsApplicationPlaying))]
    [SerializeField] private bool _isOpened;

    private bool _inMotion;

    private bool IsApplicationPlaying => Application.isPlaying;
    private PlayerInteractor PlayerInteractor => PlayerObjects.Instance.PlayerInteractor;

    protected override void Awake()
    {
        UpdateDoor();
        base.Awake();
    }

    private void OnValidate()
    {
        UpdateDoor();
    }

    protected override void ShowPromptAndOutline()
    {
        if (!_inMotion) base.ShowPromptAndOutline();
    }

    protected override void Interact()
    {
        if (!_inMotion)
        {
            HidePromptAndOutline();
            SwitchDoorAnimated();
        }
    }

    public void SetOpened(bool opened)
    {
        _isOpened = opened;
        UpdateDoor();
    }

    public void SwitchDoorAnimated()
    {
        if (_inMotion)
        {
            Debug.LogWarning("Door already in animation!");
            return;
        }

        RuntimeManager.PlayOneShotAttached(FmodEvents.Instance.SPT_MovingDoor, _doorTransform.gameObject);

        Vector3 targetRotation;
        if (_isOpened) targetRotation = new Vector3(0, 0, 0);
        else targetRotation = new Vector3(0, _openedYRotation, 0);

        _inMotion = true;
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(0.1f);
        sequence.Append(_doorTransform.DOLocalRotate(targetRotation, 1.5f).SetEase(Ease.InOutSine));
        sequence.AppendInterval(0.1f);

        sequence.OnComplete(() =>
        {
            _inMotion = false;

            _isOpened = !_isOpened;

            if (PlayerInteractor.PointedInteractable == _interactionHitbox)
            {
                ShowPromptAndOutline();
            }
        });
    }

    private void UpdateDoor()
    {
        Vector3 rotation;

        if (_isOpened) rotation = new Vector3(0, _openedYRotation, 0);
        else rotation = new Vector3(0, 0, 0);

        _doorTransform.localRotation = Quaternion.Euler(rotation);
    }
}
