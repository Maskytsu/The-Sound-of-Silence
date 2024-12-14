using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTutorial : MonoBehaviour
{
    [HideInInspector] public ItemType ItemType;

    private Dictionary<ItemType, ItemInfo> ItemsPerType => ItemManager.Instance.ItemsPerType;

    private void Start()
    {
        foreach(ItemTutorial tutorial in FindObjectsOfType<ItemTutorial>())
        {
            if (tutorial != this) Destroy(tutorial.gameObject);
        }

        StartCoroutine(DestroyTutorialAfterTime());
    }

    private void Update()
    {
        ManageDestroyingTutorial();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void ManageDestroyingTutorial()
    {
        if (ItemsPerType[ItemType].GrabItemInput.WasPerformedThisFrame())
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyTutorialAfterTime()
    {
        yield return new WaitForSeconds(120f);
        Destroy(gameObject);
    }
}
