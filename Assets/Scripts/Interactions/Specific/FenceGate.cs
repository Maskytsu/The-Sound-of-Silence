using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class FenceGate : Interactable
{
    [Space]
    [SerializeField] private Transform _gateTransform;
    [Space]
    [SerializeField] private Transform _handleTransform;
    [SerializeField] private Vector3 _handlePressedRotation = new(0, 0, 30.0f);
    [Space]
    [SerializeField] private UnityEvent OnOpenedByAnimationUE = new();
    [SerializeField] private UnityEvent OnClosedByAnimationUE = new();

    private float _handlePressDuration = 0.5f;

    private PlayerInteractor _playerInteractor;
    private bool _isOpened;
    private bool _inMotion;

    private void Start()
    {
        _playerInteractor = PlayerObjects.Instance.PlayerInteractor;
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
        RuntimeManager.PlayOneShotAttached(FmodEvents.Instance.SPT_FenceGate, _gateTransform.gameObject);

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
        sequence.Join(_handleTransform.DOLocalRotate(_handlePressedRotation, _handlePressDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        sequence.AppendInterval(0.1f);

        sequence.OnComplete(() => 
        {
            _inMotion = false;
            _isOpened = !_isOpened;

            if (_isOpened) OnOpenedByAnimationUE?.Invoke();
            else OnClosedByAnimationUE?.Invoke();

            if (_playerInteractor.PointedInteractable == _interactionHitbox)
            {
                ShowPromptAndOutline();
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
