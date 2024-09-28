using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class GlassOfWater : Interactable
{
    public Action OnInteract;

    [SerializeField] private QuestScriptable _drinkQuest;

    public override void Interact()
    {
        OnInteract?.Invoke();
        //could make animation in which water disapears from glass instead of whole glass
        _drinkQuest.EndQuest();
        Destroy(transform.parent.gameObject);
    }
}
