using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerEquipment;

public class PickableItem : Interactable
{
    [SerializeField] private ItemType itemType;

    private PlayerEquipment playerEquipment;

    private void Awake()
    {
        playerEquipment = PlayerManager.Instance.PlayerEquipment;
    }

    public override void Interact()
    {
        if (itemType == ItemType.Phone) playerEquipment.havePhone = true;
        else if (itemType == ItemType.Flashlight) playerEquipment.haveFlashlight = true;
        else if (itemType == ItemType.Keys) playerEquipment.haveKeys = true;
        else if (itemType == ItemType.Shotgun) playerEquipment.haveShotgun = true;

        Destroy(transform.parent.gameObject);
    }
}
