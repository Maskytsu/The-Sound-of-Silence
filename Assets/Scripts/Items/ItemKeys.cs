using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKeys : Item
{
    private PlayerInteractor _playerInteractor;

    private void Start()
    {
        _playerInteractor = PlayerObjects.Instance.PlayerInteractor;
    }

    public override void UseItem()
    {
        if (_playerInteractor.PointedUnlockable != null)
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.KeysUnlock);
            _playerInteractor.PointedUnlockable.OnInteract?.Invoke();
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.KeysRumble);
        }
    }
}