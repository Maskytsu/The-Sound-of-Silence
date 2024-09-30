using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomCabinet : Interactable
{
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _openedYRotation;

    private PlayerInteractor _playerInteractor;
    private bool _opened;
    private bool _inMotion;

    private void Start()
    {
        _playerInteractor = PlayerManager.Instance.PlayerInteractor;
    }

    public override void ShowPrompt()
    {
        if (!_inMotion) _promptInteract.enabled = true;
    }

    public override void Interact()
    {
        if (!_inMotion)
        {
            HidePrompt();
            StartCoroutine(OpenCloseDoor());
        }
    }

    private IEnumerator OpenCloseDoor()
    {
        Quaternion startingRotation;
        Quaternion targetRotation;

        if (_opened)
        {
            startingRotation = Quaternion.Euler(0, _openedYRotation, 0);
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            startingRotation = Quaternion.Euler(0, 0, 0);
            targetRotation = Quaternion.Euler(0, _openedYRotation, 0);
        }

        float timeCount = 0f;

        _inMotion = true;
        yield return new WaitForSeconds(0.1f);

        while (_doorTransform.localRotation != targetRotation)
        {
            _doorTransform.localRotation = Quaternion.Slerp(startingRotation, targetRotation, timeCount);
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
        _opened = !_opened;

        yield return new WaitForSeconds(0.1f);
        _inMotion = false;

        if (_playerInteractor.PointedInteractable == this)
        {
            ShowPrompt();
        }
    }
}
