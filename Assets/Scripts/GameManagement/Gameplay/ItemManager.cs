using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public Dictionary<ItemType, ItemInfo> ItemsPerType { get; private set; }
    public Dictionary<InputAction, ItemInfo> ItemsPerInput { get; private set; }

    [ShowNativeProperty] public bool HavePhone => Application.isPlaying && ItemsPerType[ItemType.PHONE].PlayerHasIt;
    [ShowNativeProperty] public bool HaveFlashlight => Application.isPlaying && ItemsPerType[ItemType.FLASHLIGHT].PlayerHasIt;
    [ShowNativeProperty] public bool HaveKeys => Application.isPlaying && ItemsPerType[ItemType.KEYS].PlayerHasIt;
    [ShowNativeProperty] public bool HaveShotgun => Application.isPlaying && ItemsPerType[ItemType.SHOTGUN].PlayerHasIt;
    [Space]
    [SerializeField] private Item _phonePrefab;
    [SerializeField] private Item _flashlightPrefab;
    [SerializeField] private Item _keysPrefab;
    [SerializeField] private Item _shotgunPrefab;
    [Space]
    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private InputProvider _inputProvider;

    private PlayerInputActions.PlayerCameraMapActions PlayerCameraMap => _inputProvider.PlayerCameraMap;

    private void Awake()
    {
        CreateInstance();
        CreateDictionaries();
    }

    private void CreateDictionaries()
    {
        ItemInfo freeHand = 
            new(ItemType.NONE, PlayerCameraMap.GrabItem1, null, true);
        ItemInfo phone = 
            new(ItemType.PHONE, PlayerCameraMap.GrabItem2, _phonePrefab, _sceneSetup.HavePhone);
        ItemInfo flashlight = 
            new(ItemType.FLASHLIGHT, PlayerCameraMap.GrabItem3, _flashlightPrefab, _sceneSetup.HaveFlashlight);
        ItemInfo keys = 
            new(ItemType.KEYS, PlayerCameraMap.GrabItem4, _keysPrefab, _sceneSetup.HaveKeys);
        ItemInfo shotgun = 
            new(ItemType.SHOTGUN, PlayerCameraMap.GrabItem5, _shotgunPrefab, _sceneSetup.HaveShotgun);


        ItemsPerType = new Dictionary<ItemType, ItemInfo>()
        {
            { freeHand.ItemType, freeHand },
            { phone.ItemType, phone },
            { flashlight.ItemType, flashlight },
            { keys.ItemType, keys },
            { shotgun.ItemType, shotgun },
        };

        ItemsPerInput = new Dictionary<InputAction, ItemInfo>()
        {
            { freeHand.GrabItemInput, freeHand },
            { phone.GrabItemInput, phone },
            { flashlight.GrabItemInput, flashlight },
            { keys.GrabItemInput, keys },
            { shotgun.GrabItemInput, shotgun },
        };
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Items in the scene.");
        }
        Instance = this;
    }
}

public class ItemInfo
{
    public ItemType ItemType;
    public InputAction GrabItemInput;
    public Item ItemPrefab;
    public bool PlayerHasIt;

    public ItemInfo(ItemType itemType, InputAction grabItemInput, Item itemPrefab, bool playerHasIt)
    {
        ItemType = itemType;
        GrabItemInput = grabItemInput;
        ItemPrefab = itemPrefab;
        PlayerHasIt = playerHasIt;
    }
}

public enum ItemType
{
    NONE,
    PHONE,
    FLASHLIGHT,
    KEYS,
    SHOTGUN
}