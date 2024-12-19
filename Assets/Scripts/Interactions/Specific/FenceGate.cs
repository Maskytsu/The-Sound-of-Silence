using DG.Tweening;
using UnityEngine;

public class FenceGate : Interactable
{
    [Space]
    [SerializeField] private Transform _gateTransform;

    private PlayerInteractor _playerInteractor;
    private bool _isOpened;
    private bool _inMotion;

    private void Start()
    {
        _playerInteractor = PlayerObjects.Instance.PlayerInteractor;
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
            OpenCloseGate();
        }
    }

    public void SetOpened(bool opened)
    {
        _isOpened = opened;
        UpdateGate();
    }

    private void OpenCloseGate()
    {
        Vector3 targetRotation;

        if (_isOpened)
        {
            targetRotation = new Vector3(0, 0, 0);
        }
        else
        {
            targetRotation = new Vector3(0, 75, 0);
        }

        _inMotion = true;
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(0.1f);
        sequence.Append(_gateTransform.DOLocalRotate(targetRotation, 1.5f).SetEase(Ease.InOutSine));
        sequence.AppendInterval(0.1f);

        sequence.OnComplete(() => 
        {
            _inMotion = false;

            _isOpened = !_isOpened;

            if (_playerInteractor.PointedInteractable == _interactionHitbox)
            {
                ShowPrompt();
            }
        });
    }

    private void UpdateGate()
    {
        Vector3 rotation;

        if (_isOpened) rotation = new Vector3(0, 75, 0);
        else rotation = new Vector3(0, 0, 0);

        _gateTransform.localRotation = Quaternion.Euler(rotation);
    }
}
