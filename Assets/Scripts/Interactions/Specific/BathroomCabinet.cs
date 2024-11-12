using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomCabinet : Interactable
{
    [Space]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _openedYRotation;

    private PlayerInteractor _playerInteractor;
    private bool _opened;
    private bool _inMotion;

    private void Start()
    {
        _playerInteractor = PlayerObjectsHolder.Instance.PlayerInteractor;
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
            OpenCloseDoor();
        }
    }

    private void OpenCloseDoor()
    {
        Vector3 targetRotation;

        if (_opened)
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

            _opened = !_opened;

            if (_playerInteractor.PointedInteractable == InteractionHitbox)
            {
                ShowPrompt();
            }
        });
    }
}
