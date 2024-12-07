using System.Collections.Generic;
using UnityEngine;

public class ItemTutorial : MonoBehaviour
{
    [HideInInspector] public ItemType ItemType;

    private Dictionary<ItemType, ItemInfo> ItemsPerType => ItemManager.Instance.ItemsPerType;

    private void Update()
    {
        ManageDestroyingTutorial();
    }

    private void ManageDestroyingTutorial()
    {
        if (ItemsPerType[ItemType].GrabItemInput.WasPerformedThisFrame())
        {
            Destroy(gameObject);
        }
    }
}
