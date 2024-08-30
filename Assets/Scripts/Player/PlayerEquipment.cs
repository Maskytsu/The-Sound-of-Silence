using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public bool handsAreEmpty { get; private set; }

    [Header("GameObjects")]
    [SerializeField] private Transform mainCameraTransform;

    [Header("Item Prefabs")]
    [SerializeField] private GameObject phonePrefab;
    [SerializeField] private GameObject flashlightPrefab;
    [SerializeField] private GameObject keysPrefab;
    [SerializeField] private GameObject shotgunPrefab;

    [Header("Equipment")]
    public bool havePhone = false;
    public bool haveFlashlight = false;
    public bool haveKeys = false;
    public bool haveShotgun = false;

    private PlayerInputActions playerInputActions;

    private Dictionary<ItemType, GameObject> items;
    private ItemType itemInHand = ItemType.None;
    private GameObject spawnedItemInHand;
    private bool ableToChangeOrUseItem = true;

    public enum ItemType
    {
        None,
        Phone,
        Flashlight,
        Keys,
        Shotgun
    }

    public void Awake()
    {
        playerInputActions = new PlayerInputActions();

        handsAreEmpty = true;

        items = new Dictionary<ItemType, GameObject>()
        {
            { ItemType.None, null },
            { ItemType.Phone, phonePrefab },
            { ItemType.Flashlight, flashlightPrefab },
            { ItemType.Keys, keysPrefab },
            { ItemType.Shotgun, shotgunPrefab },
        };
    }
    public void Update()
    {
        ManageInputs();
    }

    private void ManageInputs()
    {
        if (playerInputActions.PlayerMap.GrabItem1.WasPerformedThisFrame() && ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.None));
        }
        else if (playerInputActions.PlayerMap.GrabItem2.WasPerformedThisFrame() && havePhone && ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.Phone));
        }
        else if (playerInputActions.PlayerMap.GrabItem3.WasPerformedThisFrame() && haveFlashlight && ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.Flashlight));
        }
        else if (playerInputActions.PlayerMap.GrabItem4.WasPerformedThisFrame() && haveKeys && ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.Keys));
        }
        else if (playerInputActions.PlayerMap.GrabItem5.WasPerformedThisFrame() && havePhone && ableToChangeOrUseItem)
        {
            StartCoroutine(ChangeItem(ItemType.Shotgun));
        }


        if (playerInputActions.PlayerMap.UseItem.WasPerformedThisFrame() && itemInHand != ItemType.None && ableToChangeOrUseItem)
        {
            spawnedItemInHand.GetComponent<Item>().UseItem();
        }
    }

    private IEnumerator ChangeItem(ItemType chosenItem)
    {
        if (chosenItem == itemInHand) yield break;

        ableToChangeOrUseItem = false;

        if (itemInHand != ItemType.None)
        {
            while (true)
            {
                //there will be animation of puting item to pocket
                yield return 0;
                break;
            }
            Destroy(spawnedItemInHand);
            spawnedItemInHand = null;
        }

        if (chosenItem != ItemType.None)
        {
            handsAreEmpty = false;
            spawnedItemInHand = Instantiate(items[chosenItem], mainCameraTransform);
            spawnedItemInHand.transform.localPosition = new Vector3(0.35f, -0.25f, 0.5f);

            while (true)
            {
                //there will be animation of grabbing item
                yield return 0;
                break;
            }
        }
        else handsAreEmpty = true;


        itemInHand = chosenItem;
        ableToChangeOrUseItem = true;
    }

    private void OnEnable()
    {
        playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.PlayerMap.Disable();
    }
}
