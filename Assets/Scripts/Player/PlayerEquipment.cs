using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    public bool HandsAreEmpty { get; private set; }
    public Item SpawnedItemInHand { get; private set; }

    private ItemType _itemInHand;

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

    public void ChangeItem(ItemType chosenItem)
    {
        if (chosenItem == _itemInHand) return;
        if (!ItemsPerType[chosenItem].PlayerHasIt) return;

        if (_itemInHand != ItemType.NONE)
        {
            Destroy(SpawnedItemInHand.gameObject);
            SpawnedItemInHand = null;
        }

        if (chosenItem != ItemType.NONE)
        {
            HandsAreEmpty = false;
            SpawnedItemInHand = Instantiate(ItemsPerType[chosenItem].ItemPrefab, CameraBrainPos);
            //SpawnedItemInHand.transform.localPosition = new Vector3(0.35f, -0.25f, 0.5f);
            SpawnedItemInHand.transform.localPosition = ItemsPerType[chosenItem].ItemPrefab.transform.position;
            SpawnedItemInHand.transform.localRotation = ItemsPerType[chosenItem].ItemPrefab.transform.rotation;
        }
        else
        {
            HandsAreEmpty = true;
        }

        _itemInHand = chosenItem;
    }

    private void ManageInputs()
    {
        foreach (KeyValuePair<InputAction, ItemInfo> item in ItemsPerInput)
        {
            if (item.Key.WasPerformedThisFrame())
            {
                ChangeItem(item.Value.ItemType);
                break;
            }
        }

        if (PlayerCameraMap.UseItem.WasPerformedThisFrame() && _itemInHand != ItemType.NONE)
        {
            SpawnedItemInHand.UseItem();
        }
    }


    /*
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
            Destroy(SpawnedItemInHand.gameObject);
            SpawnedItemInHand = null;
        }

        if (chosenItem != ItemType.NONE)
        {
            HandsAreEmpty = false;
            SpawnedItemInHand = Instantiate(ItemsPerType[chosenItem].ItemPrefab, CameraBrainPos);
            SpawnedItemInHand.transform.localPosition = new Vector3(0.35f, -0.25f, 0.5f);

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
    */
}