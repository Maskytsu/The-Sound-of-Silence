using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Door : Interactable
{
    [Space]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _openedYRotation;
    [DisableIf(nameof(IsApplicationPlaying))]
    [SerializeField] private bool _isOpened;

    private PlayerInteractor _playerInteractor;
    private bool _inMotion;

    private bool IsApplicationPlaying => Application.isPlaying;

    protected override void Awake()
    {
        UpdateDoor();
        AssignMethodsToEvents();
    }

    private void Start()
    {
        _playerInteractor = PlayerManager.Instance.PlayerInteractor;
    }

    private void OnValidate()
    {
        UpdateDoor();
    }

    protected override void ShowPrompt()
    {
        if (!_inMotion) _promptInteract.enabled = true;
    }

    protected override void Interact()
    {
        if (!_inMotion)
        {
            HidePrompt();
            SwitchDoorAnimated();
        }
    }

    private void SwitchDoorAnimated()
    {
        Vector3 targetRotation;

        if (_isOpened)
        {
            targetRotation = new Vector3(0, 0, 0);
        }
        else
        {
            targetRotation = new Vector3(0, _openedYRotation, 0);
        }

        _inMotion = true;
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(0.1f);
        sequence.Append(_doorTransform.DOLocalRotate(targetRotation, 1.5f).SetEase(Ease.InOutSine));
        sequence.AppendInterval(0.1f);

        sequence.OnComplete(() =>
        {
            _inMotion = false;

            _isOpened = !_isOpened;

            if (_playerInteractor.PointedInteractable == InteractionHitbox)
            {
                ShowPrompt();
            }
        });
    }

    private void UpdateDoor()
    {
        Vector3 rotation;

        if (_isOpened)
        {
            rotation = new Vector3(0, _openedYRotation, 0);
        }
        else
        {
            rotation = new Vector3(0, 0, 0);
        }

        _doorTransform.localRotation = Quaternion.Euler(rotation);
    }
}
