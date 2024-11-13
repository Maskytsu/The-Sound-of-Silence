using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    public bool HandsAreEmpty { get; private set; }

    private ItemType _itemInHand;
    private Item _spawnedItemInHand;
    private bool _ableToChangeOrUseItem = true;

    private Transform CameraBrainPos => CameraManager.Instance.CameraBrain.transform;

    private PlayerInputActions.PlayerCameraMapActions PlayerCameraMap => InputProvider.Instance.PlayerCameraMap;
    private Dictionary<ItemType, ItemInfo> ItemsPerType => ItemManager.Instance.ItemsPerType;
    private Dictionary<InputAction, ItemInfo> ItemsPerInput => ItemManager.Instance.ItemsPerInput;

    public void Awake()
    {
        HandsAreEmpty = true;
        _itemInHand = ItemType.NONE;
    }

    private void Update()
    {
        ManageInputs();
    }

    public IEnumerator ChangeItem(ItemType chosenItem)
    {
        if (chosenItem == _itemInHand) yield break;

        _ableToChangeOrUseItem = false;

        if (_itemInHand != ItemType.NONE)
        {
            while (true)
            {
                //there will be animation of puting item to pocket
                yield return 0;
                break;
            }
            Destroy(_spawnedItemInHand.gameObject);
            _spawnedItemInHand = null;
        }

        if (chosenItem != ItemType.NONE)
        {
            HandsAreEmpty = false;
            _spawnedItemInHand = Instantiate(ItemsPerType[chosenItem].ItemPrefab, CameraBrainPos);
            _spawnedItemInHand.transform.localPosition = new Vector3(0.35f, -0.25f, 0.5f);

            while (true)
            {
                //there will be animation of grabbing item
                yield return 0;
                break;
            }
        }
        else HandsAreEmpty = true;


        _itemInHand = chosenItem;
        _ableToChangeOrUseItem = true;
    }

    private void ManageInputs()
    {
        InputAction pressedInput = null;

        if (PlayerCameraMap.GrabItem1.WasPerformedThisFrame()) pressedInput = PlayerCameraMap.GrabItem1;
        else if (PlayerCameraMap.GrabItem2.WasPerformedThisFrame()) pressedInput = PlayerCameraMap.GrabItem2;
        else if (PlayerCameraMap.GrabItem3.WasPerformedThisFrame()) pressedInput = PlayerCameraMap.GrabItem3;
        else if (PlayerCameraMap.GrabItem4.WasPerformedThisFrame()) pressedInput = PlayerCameraMap.GrabItem4;
        else if (PlayerCameraMap.GrabItem5.WasPerformedThisFrame()) pressedInput = PlayerCameraMap.GrabItem5;

        if (pressedInput != null && _ableToChangeOrUseItem && ItemsPerInput[pressedInput].PlayerHasIt)
        {
            StartCoroutine(ChangeItem(ItemsPerInput[pressedInput].ItemType));
        }

        if (PlayerCameraMap.UseItem.WasPerformedThisFrame() && _itemInHand != ItemType.NONE && _ableToChangeOrUseItem)
        {
            _spawnedItemInHand.UseItem();
        }
    }
}