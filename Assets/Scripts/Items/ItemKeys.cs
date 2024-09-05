using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKeys : Item
{
    private PlayerInteractor _playerInteractor;

    private void Start()
    {
        _playerInteractor = PlayerManager.Instance.PlayerInteractor;
    }

    public override void UseItem()
    {
        _playerInteractor.PointedUnlockable.Unlock();
    }
}