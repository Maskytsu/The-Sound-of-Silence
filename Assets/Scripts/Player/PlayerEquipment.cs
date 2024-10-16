using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public bool HandsAreEmpty { get; private set; }

    [Header("Equipment")]
    public bool HavePhone = false;
    public bool HaveFlashlight = false;
    public bool HaveKeys = false;
    public bool HaveShotgun = false;

    [Header("Item Prefabs")]
    [SerializeField] private GameObject _phonePrefab;
    [SerializeField] private GameObject _flashlightPrefab;
    [SerializeField] private GameObject _keysPrefab;
    [SerializeField] private GameObject _shotgunPrefab;

    private Transform _cameraBrain;
    private PlayerInputActions.PlayerMovementMapActions _playerMovementMap;
    private PlayerInputActions.PlayerMainMapActions _playerMainMap;

    private Dictionary<ItemType, GameObject> _items;
    private ItemType _itemInHand;
    private GameObject _spawnedItemInHand;
    private bool _ableToChangeOrUseItem = true;

    public enum ItemType
    {
        NONE,
        PHONE,
        FLASHLIGHT,
        KEYS,
        SHOTGUN
    }

    public void Awake()
    {
        HandsAreEmpty = true;
        _itemInHand = ItemType.NONE;

        _items = new Dictionary<ItemType, GameObject>()
        {
            { ItemType.NONE, null },
            { ItemType.PHONE, _phonePrefab },
            { ItemType.FLASHLIGHT, _flashlightPrefab },
            { ItemType.KEYS, _keysPrefab },
            { ItemType.SHOTGUN, _shotgunPrefab },
        };
    }

    private void Start()
    {
        _cameraBrain = PlayerManager.Instance.CameraBrain.transform;
        _playerMovementMap = InputProvider.Instance.PlayerMovementMap;
        _playerMainMap = InputProvider.Instance.PlayerMainMap;
    }

    public void Update()
    {
        ManageInputs();
    }

    private void ManageInputs()
    {
        if (_playerMainMap.GrabItem1.WasPerformedThisFrame() && _ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.NONE));
        }
        else if (_playerMainMap.GrabItem2.WasPerformedThisFrame() && HavePhone && _ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.PHONE));
        }
        else if (_playerMainMap.GrabItem3.WasPerformedThisFrame() && HaveFlashlight && _ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.FLASHLIGHT));
        }
        else if (_playerMainMap.GrabItem4.WasPerformedThisFrame() && HaveKeys && _ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.KEYS));
        }
        else if (_playerMainMap.GrabItem5.WasPerformedThisFrame() && HavePhone && _ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.SHOTGUN));
        }


        if (_playerMainMap.UseItem.WasPerformedThisFrame() && _itemInHand != ItemType.NONE && _ableToChangeOrUseItem)
        {
            _spawnedItemInHand.GetComponent<Item>().UseItem();
        }
    }

    private IEnumerator ChangeItem(ItemType chosenItem)
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
            Destroy(_spawnedItemInHand);
            _spawnedItemInHand = null;
        }

        if (chosenItem != ItemType.NONE)
        {
            HandsAreEmpty = false;
            _spawnedItemInHand = Instantiate(_items[chosenItem], _cameraBrain);
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
}
