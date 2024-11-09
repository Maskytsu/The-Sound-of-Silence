using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerEquipment;

public class PickableItem : Interactable
{
    [Space]
    [SerializeField] private ItemType _itemType;

    private PlayerEquipment _playerEquipment;

    private void Start()
    {
        _playerEquipment = PlayerManager.Instance.PlayerEquipment;
    }

    protected override void Interact()
    {
        if (_itemType == ItemType.PHONE) _playerEquipment.HavePhone = true;
        else if (_itemType == ItemType.FLASHLIGHT) _playerEquipment.HaveFlashlight = true;
        else if (_itemType == ItemType.KEYS) _playerEquipment.HaveKeys = true;
        else if (_itemType == ItemType.SHOTGUN) _playerEquipment.HaveShotgun = true;

        Destroy(gameObject);
    }
}
