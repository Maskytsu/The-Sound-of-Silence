using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTutorial : TutorialOverlay
{
    [HideInInspector] public ItemType ItemType;

    private Dictionary<ItemType, ItemInfo> ItemsPerType => ItemManager.Instance.ItemsPerType;

    protected override void Start()
    {
        base.Start();
        foreach(ItemTutorial tutorial in FindObjectsByType<ItemTutorial>(FindObjectsSortMode.None))
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
            EndTutorial();
        }
    }

    private IEnumerator DestroyTutorialAfterTime()
    {
        yield return new WaitForSeconds(120f);
        EndTutorial();
    }
}
